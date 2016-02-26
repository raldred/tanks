// Unity Engine
using UnityEngine;

public class TankTurret
{
	/// <summary>
	/// Tank reference
	/// </summary>
	Tank tank;

	/// <summary>
	/// State of the turret of the tank
    /// </summary>
	Transform turretTransform;

	/// <summary>
	/// Turret State
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
	/// Constructor
	/// </summary>
	public TankTurret(Tank parent, Transform t)
	{
		tank = parent;
		turretTransform = t;
	}

    /// <summary>
	/// Custom Start Method
    /// </summary>
	public void Start()
    {
		turretState = TankTurretState.Idle;
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
					turretTransform.localRotation = endAngleQ;
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
	}

	/// <summary>
	/// Shoot in the specified direction
	/// </summary>
	public void Shoot(Vector3 dir)
	{
		// Get the delta angle to rotate the turret
		float turretAngle = Vector3.Angle(turretTransform.forward, dir);

		Debug.LogFormat("turretAngle: {0}", turretAngle);

		turretState = TankTurretState.Firing;
	}
}
