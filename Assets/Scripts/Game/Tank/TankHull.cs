using UnityEngine;
using System.Collections;

public class TankHull
{
	/// <summary>
	/// Tank reference
	/// </summary>
	Tank tank;

    /// <summary>
	/// State of the hull of the tank
    /// </summary>
    TankHullState hullState;

	/// <summary>
	/// Hull Transform
	/// </summary>
	Transform hullTransform;

	/// <summary>
	/// Time accumulator
    /// </summary>     
	public float accumTime;

    /// <summary>
	/// Move start position
    /// </summary>
    public Vector3 startPos;
    
    /// <summary>
	/// Move target position
    /// </summary>
    public Vector3 targetPos;

    /// <summary>
	/// Move Direction
    /// </summary>
    public Vector3 moveDir;
    
    /// <summary>
	/// Current distance from current pos to target pos
    /// </summary>
    public float prevDistance;

    /// <summary>
    /// Specified rotation quaternion target
    /// </summary>
	public Quaternion endAngleQ;

	/// <summary>
	/// Time that will take rotate the object the specified degress
	/// </summary>
	public float rotationTotalTime;

	/// <summary>
	/// Rotation direction. 1: CW, -1: CCW
	/// </summary>
	public float rotDirection;

	/// <summary>
	/// Constructor
	/// </summary>
	public TankHull(Tank parent, Transform t)
	{
		hullTransform = t;
		tank = parent;
	}

	/// <summary>
	/// Custom Start Method
	/// </summary>
	public void Start()
    {
	   hullState = TankHullState.Idle;
	}
	
	/// <summary>
	/// Custom Update Method
	/// </summary>
	public void Update()
    {
		float angle;

	   	switch (hullState)
       	{
            case TankHullState.Rotating:

				accumTime += Time.deltaTime * tank.hullRotationSpeed;
				angle = hullTransform.eulerAngles.y + 360.0f * Time.deltaTime * tank.hullRotationSpeed * rotDirection;

				hullTransform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

				if (accumTime > rotationTotalTime)
				{
					hullTransform.localRotation = endAngleQ;

				if (Vector3.Distance(hullTransform.position, targetPos) < 0.1f)
                    {
                    	hullState = TankHullState.Idle;
                    }
                    else
                    {
						startPos = hullTransform.position;
                        hullState = TankHullState.Moving;
                    }
				}

                break;
                
            case TankHullState.Moving:
            
				hullTransform.position = hullTransform.position + moveDir * tank.speedMovement * Time.deltaTime;

               	// Update the row/col position
				//currentRowCol = Map.Instance.WorldPosToRowCol(hullTransform.position);
                
				float dis = Vector3.Distance(hullTransform.position, targetPos);
                if (dis > prevDistance)
                {
					hullTransform.position = targetPos;
                    hullState = TankHullState.Idle;
                }
                else
                {
                    prevDistance = dis;
                }
                break;
       	}
	}
    
	/// <summary>
	/// Rotate all the tank, the specified degrees
    /// </summary>
	public void RotateRel(float angleOfs)
    {
		accumTime = 0.0f;
        hullState = TankHullState.Rotating;

		float startAngle = Angle.Normalize(hullTransform.localRotation.eulerAngles.y);
		endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(startAngle + angleOfs), 0.0f);
		rotDirection = (angleOfs > 0.0f ? 1.0f : -1.0f);
		rotationTotalTime = (Mathf.Abs(angleOfs) / 360.0f);
    }	

    /// <summary>
	/// Rotate all the tank, to the specified angle (in degrees)
    /// </summary>
    public void RotateAbs(float targetAngle)
    {
		accumTime = 0.0f;
		hullState = TankHullState.Rotating;

		float startAngle = hullTransform.eulerAngles.y;
		float dif = Angle.GetClosestAngleDif(startAngle, targetAngle);

		endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(targetAngle), 0.0f);
		rotDirection = (dif > 0.0f ? 1.0f : -1.0f);
		rotationTotalTime = (Mathf.Abs(dif) / 360.0f);
    }
}
