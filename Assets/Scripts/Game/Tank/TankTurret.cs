// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class TankTurret
{
	// Tank reference
	Tank tank;

	// Reference to the turret game object
	Transform turretTransform;

	// Reference to the turret cannon game object
	Transform cannonTransform;

	// Missile spawn point
	Transform spawnPointTransform;

	// State of the turret of the tank
	TankTurretState turretState;

	// Time accumulator
	float accumTimeTurret;

    // Specified rotation quaternion target
	Quaternion endAngleQ;

	// Time that will take rotate the object the specified degress
	float rotationTotalTime;

	// Rotation direction. 1: CW, -1: CCW
	float rotDirection;

	// Missile Manager
	ItemPoolManager missileManager;

	// Flying missile
	GameObject missile;

	// Turret sound id
	int turretSndId;

	// Constructor
	public TankTurret(Tank parent, Transform turret, Transform cannon)
	{
		tank = parent;
		turretTransform = turret;
		cannonTransform = cannon;

		spawnPointTransform = cannonTransform.FindChild("SpawnPoint");
	}

	// Custom Start Method
	public void Start()
    {
		turretState = TankTurretState.Idle;

		missileManager = PoolManager.Instance.GetItemPoolManager("MissileManager");

	}
	
	// Custom Update Method
	public void Update()
    {
		switch (turretState)
       	{
            case TankTurretState.Rotating:

				accumTimeTurret += Time.deltaTime * tank.prop.hullRotationSpeed;
				float angle = turretTransform.localRotation.eulerAngles.y + 360.0f * Time.deltaTime * tank.prop.hullRotationSpeed * rotDirection;

				turretTransform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

				if (accumTimeTurret > rotationTotalTime)
				{
					SoundManager.Instance.StopSound(turretSndId);

					turretTransform.localRotation = endAngleQ;
					turretState = TankTurretState.Idle;
				}

                break;

            case TankTurretState.Firing1:
				accumTimeTurret += Time.deltaTime * 20.0f;
				cannonTransform.localPosition = new Vector3(cannonTransform.localPosition.x, cannonTransform.localPosition.y, Mathf.Lerp(0.0f, -0.3f, accumTimeTurret));
				if (accumTimeTurret >= 1.0f)
				{
					SoundManager.Instance.PlaySound(SndId.SND_TANK_SHOOT);

					accumTimeTurret = 0.0f;
					turretState = TankTurretState.Firing2;
				}
				break;
			case TankTurretState.Firing2:
				accumTimeTurret += Time.deltaTime * 5.0f;
				cannonTransform.localPosition = new Vector3(cannonTransform.localPosition.x, cannonTransform.localPosition.y, Mathf.Lerp(-0.3f, 0.0f, accumTimeTurret));
				if (accumTimeTurret >= 1.0f)
				{
					accumTimeTurret = 0.0f;
					turretState = TankTurretState.Idle;
				}
				break;
       	}
	}

	// Rotate the turret clockwise using a relative angle
	public void RotateRel(float angleOfs)
	{
		accumTimeTurret = 0.0f;
        turretState = TankTurretState.Rotating;

		float startAngle = Angle.Normalize(turretTransform.localRotation.eulerAngles.y);
		endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(startAngle + angleOfs), 0.0f);
		rotDirection = (angleOfs > 0.0f ? 1.0f : -1.0f);
		rotationTotalTime = (Mathf.Abs(angleOfs) / 360.0f);

		turretSndId = SoundManager.Instance.PlaySound(SndId.SND_TANK_TURRET_ROTATE);
	}
    
	// Rotate the tower clockwise using an absolute angle
	public void RotateAbs(float targetAngle)
	{
		accumTimeTurret = 0.0f;
		turretState = TankTurretState.Rotating;

		float startAngle = turretTransform.eulerAngles.y;
		float dif = Angle.GetClosestAngleDif(startAngle, targetAngle);

		endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(targetAngle), 0.0f);
		rotDirection = (dif > 0.0f ? 1.0f : -1.0f);
		rotationTotalTime = (Mathf.Abs(dif) / 360.0f);

		turretSndId = SoundManager.Instance.PlaySound(SndId.SND_TANK_TURRET_ROTATE);
	}

    System.DateTime lastShootTime = System.DateTime.MinValue;
    
	// Shoot in the specified direction
	public void Shoot(Vector3 dir)
	{
        System.TimeSpan ts = System.DateTime.Now - lastShootTime;
        
        if (turretState != TankTurretState.Firing1 && ts.TotalSeconds > tank.prop.shootCoolDown)
        {
            lastShootTime = System.DateTime.Now;
            
            // Get the delta angle to rotate the turret
            float turretAngle = Vector3.Angle(turretTransform.forward, dir);

            accumTimeTurret = 0.0f;
            turretState = TankTurretState.Firing1;

            missile = missileManager.GetItem();
            missile.transform.position = spawnPointTransform.position;
            missile.transform.forward = cannonTransform.forward;
            missile.SetActive(true);
            missile.GetComponent<Missile>().SetOnMissileHit(OnMissileHit);

            tank.AI.OnShoot();
        }
        else
        {
            Debug.LogFormat("TankTurret@Shoot. Cannot shoot. Last shoot is still too recent. State: {0}", turretState);
        }
	}

    // Returns true if it's possible for the turret to shoot    
    public bool CanShoot()
    {
        System.TimeSpan ts = System.DateTime.Now - lastShootTime;
        return (turretState != TankTurretState.Firing1 && ts.TotalSeconds > tank.prop.shootCoolDown);
    }

	void OnMissileHit(Missile missile, GameObject hittedGameObject)
	{
		tank.AI.OnShootHit(hittedGameObject);
	}
    
    // Returns the info of the radar. The visible tanks are returned with this method 
    public ViewInfo GetViewInfo()
    {
        ViewInfo vi = new ViewInfo();
        
        List<Tank> tanks = TankManager.Instance.GetTanks();
        
        Vector3 pos = turretTransform.position;
        
        for (int i=0; i<tanks.Count; i++)
        {
            if (tank == tanks[i]) continue;
            
            // 1. Check the distance
            if (Vector3.Distance(pos, tanks[i].transform.position) < tank.prop.viewDistance)
            {
                // 2. Check the angle
                Vector3 p1 = tanks[i].transform.position - pos;
                if (Vector3.Angle(p1, turretTransform.forward) <= tank.prop.viewAngle / 2.0f)
                {
                    vi.AddTankInfo(tanks[i]);
                }
            }
        }

		MapObstacle[] obstacles = Map.Instance.GetObstacles();

		for (int i=0; i<obstacles.Length; i++)
        {
            // 1. Check the distance
			if (Vector3.Distance(pos, obstacles[i].transform.position) < tank.prop.viewDistance)
            {
                // 2. Check the angle
				Vector3 p1 = obstacles[i].transform.position - pos;
                if (Vector3.Angle(p1, turretTransform.forward) <= tank.prop.viewAngle / 2.0f)
                {
					vi.AddObstacleInfo(obstacles[i]);
                }
            }
        }

		Debug.LogFormat("Visible {0} of {1} tanks and {2} obstacles", vi.GetTankCount(), tanks.Count, vi.GetEntityCount() - vi.GetTankCount());

        return vi;
    }
}
