
// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

// Vectrosity
using Vectrosity;

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
	/// Movement sound clip id
	/// </summary>
	int moveSndId;

	List<KeyValuePair<int, int>> movePath;

	VectorLine vl;

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
						SoundManager.Instance.StopSound(moveSndId);

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
					SoundManager.Instance.StopSound(moveSndId);

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
		moveSndId = SoundManager.Instance.PlaySound(SndId.SND_TANK_TURRET_ROTATE);

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
		moveSndId = SoundManager.Instance.PlaySound(SndId.SND_TANK_TURRET_ROTATE);

		accumTime = 0.0f;
		hullState = TankHullState.Rotating;

		float startAngle = hullTransform.eulerAngles.y;
		float dif = Angle.GetClosestAngleDif(startAngle, targetAngle);

		endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(targetAngle), 0.0f);
		rotDirection = (dif > 0.0f ? 1.0f : -1.0f);
		rotationTotalTime = (Mathf.Abs(dif) / 360.0f);
    }

	/// <summary>
	/// Move the tank at full speed to the specified world position
	/// </summary>
	public void MoveToWorldPos(Vector3 pos)
	{
		if (Vector3.Distance(hullTransform.position, pos) < 0.1f) return;

        targetPos = pos;
		prevDistance = Vector3.Distance(hullTransform.position, targetPos); 
        
		CollectRowColPath(hullTransform.position, pos);
        DrawMovePath();
        
        // Get the normalized move direction
		moveDir = Vector3.Normalize(pos - hullTransform.position);
        
        // Get the angle difference between the moving direction and the facing of the tank
		float angleDif = Vector3.Angle(hullTransform.forward, moveDir);
        
        if (angleDif > 0.1f)
        {
            RotateRel(angleDif);
        }
        else
        {
            Debug.LogFormat("Already looking in the right direction");

			moveSndId = SoundManager.Instance.PlaySound(SndId.SND_TANK_ENGINE);
        }
        
        //vl = VectorLine.SetLine3D(debugLineColor, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), new Vector3(moveVars.targetPos.x, moveVars.targetPos.y + 0.1f, moveVars.targetPos.z));
        //vl.SetWidth(2.0f);
        //vl.Draw3D();
	}

	/// <summary>
	/// Move the tank at full speed to the specified row/col map position. Returns false
	/// if it's not possible to move the tank to that position.
	/// </summary>
	public bool MoveToRowCol(int row, int col)
	{
		if (Map.Instance.CheckPos(row, col))
		{
			MoveToWorldPos(Map.Instance.RowColToWorldPos(row, col));
			return true;
		}
		else
			return false;
	}

	void CollectRowColPath(Vector3 startPos, Vector3 endPos)
    {
        movePath = new List<KeyValuePair<int, int>>();
        
        Vector3 dir = Vector3.Normalize(endPos - startPos);
        
        float distance = Vector3.Distance(startPos, endPos);
        
        int steps = (int) Mathf.Ceil(distance / Map.Instance.cellSize);
        
        for (int i=1; i<steps; i++)
        {
            Vector3 newPos = startPos + dir * Map.Instance.cellSize * i;
            
            KeyValuePair<int, int> rowcol = Map.Instance.WorldPosToRowCol(newPos);
            
            if (!movePath.Contains(rowcol))
            {
                movePath.Add(rowcol);
            }
        }
    }

	List<VectorLine> movePathLines;
    void DrawMovePath()
    {
        if (movePathLines != null)
        {
            for (int i=0; i<movePathLines.Count; i++)
            {
                VectorLine vl = movePathLines[i];
                VectorLine.Destroy(ref vl);    
            }
        }
        
        movePathLines = new List<VectorLine>();
        
        for (int i=0; i<movePath.Count; i++)
		{
            VectorLine vlo = new VectorLine("path", new List<Vector3>(5), null, 2.0f, LineType.Continuous);
            vlo.SetColor(tank.debugLineColor);
            vlo.MakeRect(Map.Instance.MapData[movePath[i].Key, movePath[i].Value].Pos + new Vector3(Map.Instance.cellSize / -2.0f, 0.11f, Map.Instance.cellSize / -2.0f),
                         Map.Instance.MapData[movePath[i].Key, movePath[i].Value].Pos + new Vector3(Map.Instance.cellSize / 2.0f, 0.11f, Map.Instance.cellSize / 2.0f));
                                 
            movePathLines.Add(vlo);
        }
        
        for (int i=0; i<movePathLines.Count; i++)
        {
            movePathLines[i].Draw3D();    
        }
    }
}
