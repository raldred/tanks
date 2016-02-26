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
	/// The turrent transform
    /// </summary>
    public Transform turret;
    
    /// <summary>
	/// Tank Property. Hull Rotation Speed
    /// </summary>
    public float hullRotationSpeed = 1.0f;
    
	/// <summary>
	/// Tank Property. Movement Speed
    /// </summary>
	public float speedMovement = 5.0f;
    
    /// State of the hull of the tank
    TankHullState hullState;

    /// <summary>
    ///  State of the turret
    /// </summary>
    TankTurretState turretState;
    
    // Current position of the tank in row/col
    KeyValuePair<int, int> currentRowCol;
    
    struct MoveVars
    {   
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
    }
    
    struct TurretVars
    {
		/// <summary>
		/// Time accumulator
        /// </summary>     
		public float accumTimeTurret;

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
    }
    
    List<KeyValuePair<int, int>> movePath;
    
    MoveVars moveVars = new MoveVars();
    
    TurretVars turretVars = new TurretVars();
    
    VectorLine vl;

    
	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		hullState = TankHullState.Idle;
       	turretState = TankTurretState.Idle;

		if (turret == null)
		{
			turret = getTurretReference();
		}
	}

	/// <summary>
	/// Get the turrent reference
	/// </summary>
	/// <returns>The turret reference</returns>
	Transform getTurretReference()
	{
		return transform.FindChild("Turret");
	}
	
	/// <summary>
	/// Unity Update Method
	/// </summary>
	void Update()
	{
		float angle;

	   	switch (hullState)
       	{
            case TankHullState.Rotating:

				moveVars.accumTime += Time.deltaTime * hullRotationSpeed;
				angle = transform.eulerAngles.y + 360.0f * Time.deltaTime * hullRotationSpeed * moveVars.rotDirection;

				transform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

				if (moveVars.accumTime > moveVars.rotationTotalTime)
				{
					transform.localRotation = moveVars.endAngleQ;

					if (Vector3.Distance(transform.position, moveVars.targetPos) < 0.1f)
                    {
                    	hullState = TankHullState.Idle;
                    }
                    else
                    {
                        moveVars.startPos = transform.position;
                        hullState = TankHullState.Moving;
                    }
				}

                break;
                
            case TankHullState.Moving:
            
               transform.position = transform.position + moveVars.moveDir * speedMovement * Time.deltaTime;

               // Update the row/col position
               currentRowCol = Map.Instance.WorldPosToRowCol(transform.position);
                
                float dis = Vector3.Distance(transform.position, moveVars.targetPos);
                if (dis > moveVars.prevDistance)
                {
                    transform.position = moveVars.targetPos;
                    hullState = TankHullState.Idle;
                }
                else
                {
                    moveVars.prevDistance = dis;
                }
                break;
       	}

       	switch (turretState)
       	{
            case TankTurretState.Rotating:

				turretVars.accumTimeTurret += Time.deltaTime * hullRotationSpeed;
				angle = turret.transform.localRotation.eulerAngles.y + 360.0f * Time.deltaTime * hullRotationSpeed * turretVars.rotDirection;

				turret.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

				if (turretVars.accumTimeTurret > turretVars.rotationTotalTime)
				{
					turret.transform.localRotation = turretVars.endAngleQ;
					turretState = TankTurretState.Idle;
				}

                break;
       	}
	}

	/// <summary>
	/// Rotate the turret clockwise using a relative angle
	/// </summary>
	public void RotateTurretRel(float angleOfs)
	{
		turretVars.accumTimeTurret = 0.0f;
        turretState = TankTurretState.Rotating;

		float startAngle = Angle.Normalize(turret.transform.localRotation.eulerAngles.y);
		turretVars.endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(startAngle + angleOfs), 0.0f);
		turretVars.rotDirection = (angleOfs > 0.0f ? 1.0f : -1.0f);
		turretVars.rotationTotalTime = (Mathf.Abs(angleOfs) / 360.0f);
	}
    
    /// <summary>
	/// Rotate the tower clockwise using an absolute angle
    /// </summary>
	public void RotateTurretAbs(float targetAngle)
	{
		turretVars.accumTimeTurret = 0.0f;
		turretState = TankTurretState.Rotating;

		float startAngle = turret.transform.eulerAngles.y;
		float dif = Angle.GetClosestAngleDif(startAngle, targetAngle);

		turretVars.endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(targetAngle), 0.0f);
		turretVars.rotDirection = (dif > 0.0f ? 1.0f : -1.0f);
		turretVars.rotationTotalTime = (Mathf.Abs(dif) / 360.0f);
	}
    
    /// <summary>
	/// Rotate all the tank, the specified degrees
    /// </summary>
	public void RotateRel(float angleOfs)
    {
		moveVars.accumTime = 0.0f;
        hullState = TankHullState.Rotating;

		float startAngle = Angle.Normalize(transform.localRotation.eulerAngles.y);
		moveVars.endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(startAngle + angleOfs), 0.0f);
		moveVars.rotDirection = (angleOfs > 0.0f ? 1.0f : -1.0f);
		moveVars.rotationTotalTime = (Mathf.Abs(angleOfs) / 360.0f);
    }	

    /// <summary>
	/// Rotate all the tank, to the specified angle (in degrees)
    /// </summary>
    public void RotateAbs(float targetAngle)
    {
		moveVars.accumTime = 0.0f;
		hullState = TankHullState.Rotating;

		float startAngle = transform.eulerAngles.y;
		float dif = Angle.GetClosestAngleDif(startAngle, targetAngle);

		moveVars.endAngleQ = Quaternion.Euler(0.0f, Angle.Normalize(targetAngle), 0.0f);
		moveVars.rotDirection = (dif > 0.0f ? 1.0f : -1.0f);
		moveVars.rotationTotalTime = (Mathf.Abs(dif) / 360.0f);
    }

	// Move the tank at full speed to the specified world position
	public void MoveToWorldPos(Vector3 pos)
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
        RotateAbs(-45.0f);
		//RotateRel(540.0f);
    }
    
    public void DebugMove3()
    {
		RotateTurretRel(180);
		//Debug.LogFormat("{0}", turret.transform.eulerAngles.y);
    }

    /// <summary>
	/// Unity OnTriggerEnter method. Called when the tank hits a trigger in the scene.
	/// The reaction will be stop the tank immediatelly (and notify somehow?)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        hullState = TankHullState.Idle;
    }


}
