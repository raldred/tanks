// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

// Vectrosity
using Vectrosity;

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

    VectorLine vl;

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

	// Move the tank at full speed to the specified world position
	/*public void MoveToWorldPos(Vector3 pos)
	{
        moveVars.targetPos = pos;
        moveVars.prevDistance = Vector3.Distance(transform.position, moveVars.targetPos); 
        
        CollectRowColPath(transform.position, pos);
        DrawMovePath();
        
        // Get the normalized move direction
        moveVars.moveDir = Vector3.Normalize(pos - transform.position);
        
        // Get the angle difference between the moving direction and the facing of the tank
        float angleDif = Vector3.Angle(transform.forward, moveVars.moveDir);
        
        if (angleDif > 0.1f)
        {
            RotateRel(angleDif);
        }
        else
        {
            Debug.LogFormat("Already looking in the right direction");
        }
        
        vl = VectorLine.SetLine3D(debugLineColor, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), new Vector3(moveVars.targetPos.x, moveVars.targetPos.y + 0.1f, moveVars.targetPos.z));
        vl.SetWidth(2.0f);
        vl.Draw3D();
	}*/

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
            vlo.SetColor(debugLineColor);
            vlo.MakeRect(Map.Instance.MapData[movePath[i].Key, movePath[i].Value].Pos + new Vector3(Map.Instance.cellSize / -2.0f, 0.11f, Map.Instance.cellSize / -2.0f),
                         Map.Instance.MapData[movePath[i].Key, movePath[i].Value].Pos + new Vector3(Map.Instance.cellSize / 2.0f, 0.11f, Map.Instance.cellSize / 2.0f));
                                 
            movePathLines.Add(vlo);
        }
        
        for (int i=0; i<movePathLines.Count; i++)
        {
            movePathLines[i].Draw3D();    
        }
    }

	// Move the tank at full speed to the specified row/col map position. Returns false
	// if it's not possible to move the tank to that position.
	public bool MoveToRowCol(int row, int col)
	{
		return false;
	}
    
    public void DebugMove1()
    {
		RotateAbs(-45.0f);
        //MoveToWorldPos(new Vector3(35f, 0f, 35f));
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
