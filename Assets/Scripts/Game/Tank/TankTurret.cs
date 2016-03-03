// Unity Engine
using UnityEngine;

public class TankTurret
{
	/// <summary>
	/// Tank reference
	/// </summary>
	Tank tank;

	/// <summary>
	/// Reference to the turret game object
    /// </summary>
	Transform turretTransform;

	/// <summary>
	/// Reference to the turret cannon game object
    /// </summary>
	Transform cannonTransform;

	/// <summary>
	/// Missile spawn point
	/// </summary>
	Transform spawnPointTransform;

	/// <summary>
	/// State of the turret of the tank
	/// </summary>
	TankTurretState turretState;

	/// <summary>
	/// Time accumulator
    /// </summary>     
	float accumTimeTurret;

	/// <summary>
    /// Specified rotation quaternion target
    /// </summary>
	Quaternion endAngleQ;

	/// <summary>
	/// Time that will take rotate the object the specified degress
	/// </summary>
	float rotationTotalTime;

	/// <summary>
	/// Rotation direction. 1: CW, -1: CCW
	/// </summary>
	float rotDirection;

	/// <summary>
	/// Missile Manager
	/// </summary>
	ItemPoolManager missileManager;

	/// <summary>
	/// Flying missile
	/// </summary>
	GameObject missile;

	/// <summary>
	/// Turret sound id
	/// </summary>
	int turretSndId;

	/// <summary>
	/// Constructor
	/// </summary>
	public TankTurret(Tank parent, Transform turret, Transform cannon)
	{
		tank = parent;
		turretTransform = turret;
		cannonTransform = cannon;

		spawnPointTransform = cannonTransform.FindChild("SpawnPoint");
	}

    /// <summary>
	/// Custom Start Method
    /// </summary>
	public void Start()
    {
		turretState = TankTurretState.Idle;

		missileManager = PoolManager.Instance.GetItemPoolManager("MissileManager");

	}
	
    /// <summary>
	/// Custom Update Method
    /// </summary>
	public void Update()
    {
		switch (turretState)
       	{
            case TankTurretState.Rotating:

				accumTimeTurret += Time.deltaTime * tank.hullRotationSpeed;
				float angle = turretTransform.localRotation.eulerAngles.y + 360.0f * Time.deltaTime * tank.hullRotationSpeed * rotDirection;

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

	/// <summary>
	/// Rotate the turret clockwise using a relative angle
	/// </summary>
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
    
    /// <summary>
	/// Rotate the tower clockwise using an absolute angle
    /// </summary>
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

	/// <summary>
	/// Shoot in the specified direction
	/// </summary>
	public void Shoot(Vector3 dir)
	{
		// Get the delta angle to rotate the turret
		float turretAngle = Vector3.Angle(turretTransform.forward, dir);

		Debug.LogFormat("turretAngle: {0}", turretAngle);

		accumTimeTurret = 0.0f;
		turretState = TankTurretState.Firing1;

		missile = missileManager.GetItem();
		missile.transform.position = spawnPointTransform.position;
		missile.transform.forward = cannonTransform.forward;
		missile.SetActive(true);
	}
}
