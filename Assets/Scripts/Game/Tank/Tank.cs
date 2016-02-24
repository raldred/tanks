// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

// Vectrosity
using Vectrosity;

public class Tank : MonoBehaviour
{
    public Color debugLineColor = Color.blue;
    
    public Transform turret;
    
    // Hull Rotation Speed
    float hullRotationSpeed = 1.0f;
    
    // Speed Movement
    float speedMovement = 1.0f;
    
    
    // State of the hull of the tank
    TankHullState hullState;
    
    TankTurretState turretState;
    
    // Time accumulator
    float accumTime;
    float accumTimeTurret;
    
    struct MoveVars
    {
        // Rotation from
        public Quaternion quatFrom;
        
        // Rotation to
        public Quaternion quatTo;
        
        // Move start position
        public Vector3 startPos;
        
        // Move target position
        public Vector3 targetPos;
        
        // Current row
        public int currentRow;
        
        // Current col
        public int currentCol;
    }
    
    struct TurretVars
    {
        // Rotation from
        public Quaternion quatFrom;
        
        // Rotation to
        public Quaternion quatTo;
    }
    
    List<KeyValuePair<int, int>> movePath;
    
    MoveVars moveVars = new MoveVars();
    
    TurretVars turretVars = new TurretVars();
    
    VectorLine vl;

    
	// Unity Start Method
	void Start()
	{
       hullState = TankHullState.Idle;
       turretState = TankTurretState.Idle;
	}
	
	// Unity Update Method
	void Update()
	{
	   switch (hullState)
       {
            case TankHullState.Rotating:
                accumTime += Time.deltaTime * hullRotationSpeed;
                transform.localRotation = Quaternion.Lerp(moveVars.quatFrom, moveVars.quatTo, accumTime);
                
                if (accumTime >= 1.0f)
                {
                        if (Vector3.Distance(transform.position, moveVars.targetPos) < 0.1f)
                        {
                            hullState = TankHullState.Idle;
                        }
                        else
                        {
                            moveVars.startPos = transform.position;
                            hullState = TankHullState.Moving;
                            accumTime = 0.0f;
                        }
                }
                break;
                
            case TankHullState.Moving:
                accumTime += Time.deltaTime * speedMovement * 0.25f;
                transform.position = Vector3.Lerp(moveVars.startPos, moveVars.targetPos, accumTime);
                
                // TODO: Update currentRow and currentCol
                
                if (accumTime >= 1.0f)
                {
                    hullState = TankHullState.Idle;
                }
                break;
       }
       
       switch (turretState)
       {
            case TankTurretState.Rotating:
                accumTimeTurret += Time.deltaTime * hullRotationSpeed;
                turret.localRotation = Quaternion.Lerp(turretVars.quatFrom, turretVars.quatTo, accumTimeTurret);
                
                if (accumTime >= 1.0f)
                {
                    turretState = TankTurretState.Idle;
                }
                break;
       }
	}

	// Rotate the turret clockwise using a relative angle
	public void RotateTurretRel(float angle)
	{
        turretState = TankTurretState.Rotating;
        accumTimeTurret = 0.0f;
        
        turretVars.quatFrom = turret.localRotation;
        turretVars.quatTo = Quaternion.Euler(0.0f, turret.eulerAngles.y + angle, 0.0f);
	}
    
    // Rotate all the tank, the specified angle
    public void RotateRel(float angle)
    {
        hullState = TankHullState.Rotating;
        accumTime = 0.0f;
        
        moveVars.quatFrom = transform.localRotation;
        moveVars.quatTo = Quaternion.Euler(0.0f, transform.eulerAngles.y + angle, 0.0f);
    }

	// Rotate the tower clockwise using an absolute angle
	public void RotateTurretAbs(float angle)
	{
		
	}

	// Move the tank at full speed to the specified world position
	public void MoveToWorldPos(Vector3 pos)
	{
        moveVars.targetPos = pos;

        CollectRowColPath(transform.position, pos);
        DrawMovePath();
        
        // Get the normalized move direction
        Vector3 dir = Vector3.Normalize(pos - transform.position);
        
        // Get the angle difference between the moving direction and the facing of the tank
        float angleDif = Vector3.Angle(transform.forward, dir);
        
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
        MoveToWorldPos(new Vector3(35f, 0f, 35f));
    }
    
    public void DebugMove2()
    {
        RotateTurretRel(45.0f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        hullState = TankHullState.Idle;
    }
}
