// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public delegate void TankEventNotification();

public class Tank : MonoBehaviour, IMissileReceptor
{
	// Color of the debug lines for this tank
    public Color debugLineColor = Color.blue;

    // Structure of properties
	public TankProperty prop = new TankProperty();

	// The turrent transform
    Transform turretTransform;

	// Reference to the turret cannon game object
	Transform turretCannon;

    // Reference to the tank hull
	TankHull hull;

	// Reference to the tank turret
    TankTurret turret;

    // Current position of the tank in row/col
    KeyValuePair<int, int> currentRowCol;

    List<KeyValuePair<int, int>> movePath;

    // Reference to the tank ai. This reference is initially null, then TankManager will call SetAI to set
    // the reference when the tank is instantiated
    TankAI tankAI;

	// Reference to the explosion manager
    ItemPoolManager explosionManager;

    // Reference to the pool manager
    ItemPoolManager flameManager;

	// Reference to the pool manager
    ItemPoolManager detonationManager;

	// Destroy timer
	float destroyTimer;

    // Returns the reference to the ai
    public TankAI AI
    {
    	get
    	{ 
			Debug.AssertFormat(tankAI != null, "Tank@AI. tankAI is null. SetAI method was not already called to set the reference of the AI in this tank ({0})", name);
    		return tankAI; 
    	}
    }

	// Unity Start Method
	void Start()
	{
		if (turretTransform == null)
		{
			turretTransform = transform.FindChild("Turret");
			if (turretTransform == null) Debug.LogError("Tank@Start. Cannot find child game object named Turret.");
		}

		if (turretCannon == null)
		{
			turretCannon = turretTransform.FindChild("Gun");
			if (turretCannon == null) Debug.LogError("Tank@Start. Cannot find child game object named Gun.");
		}

		hull = new TankHull(this, transform);
		hull.Start();

		turret = new TankTurret(this, turretTransform, turretCannon);
		turret.Start();

		if (Application.isPlaying)
        {
			explosionManager = PoolManager.Instance.GetItemPoolManager("ExplosionManager");

			flameManager = PoolManager.Instance.GetItemPoolManager("FlameManager");

			detonationManager = PoolManager.Instance.GetItemPoolManager("DetonationManager");
		}
	}

	// Unity Update Method
	void Update()
	{
		hull.Update();
		turret.Update();

		if (destroyTimer > 0.0f)
		{
			destroyTimer -= Time.deltaTime;

			if (destroyTimer <= 0.0f)
			{
				GameObject detonationItem = detonationManager.GetItem();
				detonationItem.transform.position = new Vector3(transform.position.x, 1.7f, transform.position.z);
				detonationItem.SetActive(true);
				detonationManager.RecycleItemAfterSecs(detonationItem, 3.0f);

				UnityEngine.Object.Destroy(this.gameObject);
			}
		}
	}

	// Direct hit
	public void DirectHit(float missileHitDamage, Vector3 hitPosition)
	{
		GameObject explosionItem = explosionManager.GetItem();
		explosionItem.transform.position = hitPosition;
		explosionItem.SetActive(true);
		explosionItem.GetComponent<ParticleSystem>().Play();
		explosionManager.RecycleItemAfterSecs(explosionItem, 3.0f);

		SoundManager.Instance.PlaySound(SndId.SND_EXPLOSION_OBSTACLE);

		prop.health = Mathf.Clamp(prop.health - missileHitDamage, 0.0f, float.MaxValue);

		if (prop.health == 0.0f)
		{
			GameObject flameItem = flameManager.GetItem();
			flameItem.transform.position = new Vector3(transform.position.x, 1.7f, transform.position.z);
			flameItem.SetActive(true);
			flameItem.GetComponent<ParticleSystem>().Play();
			flameManager.RecycleItemAfterSecs(flameItem, 3.0f);

			// Destroy the object in 3 seconds
			destroyTimer = 3.0f;
		}
	}

	// Area hit
	public void AreaHit(float missileHitDamage, Vector3 hitPosition)
	{
    }

	// Rotate the turret clockwise using a relative angle
	public void RotateTurretRel(float angleOfs)
	{
		turret.RotateRel(angleOfs);
	}
    
	// Rotate the tower clockwise using an absolute angle
	public void RotateTurretAbs(float targetAngle)
	{
		turret.RotateAbs(targetAngle);
	}
    
	// Rotate all the tank, the specified degrees
	public void RotateRel(float angleOfs)
    {
		hull.RotateRel(angleOfs);
    }	

	// Rotate all the tank, to the specified angle (in degrees)
    public void RotateAbs(float targetAngle)
    {
		hull.RotateAbs(targetAngle);
    }

	// Move the tank at full speed to the specified world position
	public void MoveToWorldPos(Vector3 pos)
	{
        hull.MoveToWorldPos(pos);
	}

	// Shoot in the specified direction. The y component is ignored
	public void Shoot(Vector3 dir)
	{
		turret.Shoot(dir);
	}

	// Move the tank at full speed to the specified row/col map position. Returns false
	// if it's not possible to move the tank to that position.
	public bool MoveToRowCol(int row, int col)
	{
		return hull.MoveToRowCol(row, col);
	}

    public void SetAI(TankAI ai)
    {
		tankAI = ai;
    }
    
    // Returns the info of the radar. The visible tanks are returned with this method 
    public ViewInfo GetViewInfo()
    {
        return turret.GetViewInfo();
    }
    
    // Set and changes the material of the tank
    public void SetMaterial(Material m)
    {
        
    }
}
