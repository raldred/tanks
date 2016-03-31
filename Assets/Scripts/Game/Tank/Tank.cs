// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public delegate void TankEventNotification();

public class Tank : MonoBehaviour
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
	}

	// Unity Update Method
	void Update()
	{
		hull.Update();
		turret.Update();
	}

	/// <summary>
	/// Unity OnTriggerEnter method. Called when the tank hits a trigger in the scene.
	/// The reaction will be stop the tank immediatelly (and notify somehow?)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        //hullState = TankHullState.Idle;
    }

	/// <summary>
	/// Rotate the turret clockwise using a relative angle
	/// </summary>
	public void RotateTurretRel(float angleOfs)
	{
		turret.RotateRel(angleOfs);
	}
    
    /// <summary>
	/// Rotate the tower clockwise using an absolute angle
    /// </summary>
	public void RotateTurretAbs(float targetAngle)
	{
		turret.RotateAbs(targetAngle);
	}
    
    /// <summary>
	/// Rotate all the tank, the specified degrees
    /// </summary>
	public void RotateRel(float angleOfs)
    {
		hull.RotateRel(angleOfs);
    }	

    /// <summary>
	/// Rotate all the tank, to the specified angle (in degrees)
    /// </summary>
    public void RotateAbs(float targetAngle)
    {
		hull.RotateAbs(targetAngle);
    }

	/// <summary>
	/// Move the tank at full speed to the specified world position
	/// </summary>
	public void MoveToWorldPos(Vector3 pos)
	{
        hull.MoveToWorldPos(pos);
	}

	/// <summary>
	/// Shoot in the specified direction. The y component is ignored
	/// </summary>
	public void Shoot(Vector3 dir)
	{
		turret.Shoot(dir);
	}

	/// <summary>
	/// Move the tank at full speed to the specified row/col map position. Returns false
	/// if it's not possible to move the tank to that position.
	/// </summary>
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
