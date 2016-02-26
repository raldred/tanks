// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public delegate void TankEventNotification();

public class Tank : MonoBehaviour
{
	/// <summary>
	/// Color of the debug lines for this tank
	/// </summary>
    public Color debugLineColor = Color.blue;

    /// <summary>
	/// Tank Property. Hull Rotation Speed
    /// </summary>
    public float hullRotationSpeed = 1.0f;
    
	/// <summary>
	/// Tank Property. Movement Speed
    /// </summary>
	public float speedMovement = 5.0f;
    
	/// <summary>
	/// The turrent transform
    /// </summary>
    Transform turretTransform;

    /// <summary>
    /// Reference to the tank hull
    /// </summary>
	TankHull hull;

	/// <summary>
	/// Reference to the tank turret
	/// </summary>
    TankTurret turret;

    // Current position of the tank in row/col
    KeyValuePair<int, int> currentRowCol;

    List<KeyValuePair<int, int>> movePath;

	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		if (turretTransform == null)
		{
			turretTransform = transform.FindChild("Turret");
		}

		hull = new TankHull(this, transform);
		hull.Start();

		turret = new TankTurret(this, turretTransform);
		turret.Start();
	}

	/// <summary>
	/// Unity Update Method
	/// </summary>
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

    public void DebugMove1()
    {
		//RotateAbs(-45.0f);
        //MoveToWorldPos(new Vector3(35f, 0f, 35f));
		//MoveToRowCol(-2, 4);
		Shoot(new Vector3(1.0f, 0.0f, 1.0f));
    }
    
    public void DebugMove2()
    {
		RotateRel(540.0f);
    }
    
    public void DebugMove3()
    {
		RotateTurretRel(180);
		//Debug.LogFormat("{0}", turret.transform.eulerAngles.y);
    }




}
