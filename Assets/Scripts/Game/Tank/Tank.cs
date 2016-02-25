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
    
    // Time accumulator
    float accumTime;
    float accumTimeTurret;
    
    // Current position of the tank in row/col
    KeyValuePair<int, int> currentRowCol;
    
    struct MoveVars
    {        
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
        // Rotation from
        public Quaternion quatFrom;
        
        // Rotation to
        public Quaternion quatTo;

        float faceAngle;
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
	   	switch (hullState)
       	{
            case TankHullState.Rotating:

				accumTime += Time.deltaTime * hullRotationSpeed;
				float angle = transform.eulerAngles.y + 360.0f * Time.deltaTime * hullRotationSpeed * moveVars.rotDirection;

				transform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

				if (accumTime > moveVars.rotationTotalTime)
				{
					//Debug.LogFormat("Time: {0} Angle: {1}", accumTime, transform.eulerAngles.y);

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
    
    // Rotate the tower clockwise using an absolute angle
	public void RotateTurretAbs(float angle)
	{
		turretState = TankTurretState.Rotating;
        accumTimeTurret = 0.0f;
        
        turretVars.quatFrom = turret.localRotation;
        turretVars.quatTo = Quaternion.Euler(0.0f, angle, 0.0f);
	}
    
    /// <summary>
	/// Rotate all the tank, the specified degrees
    /// </summary>
    public void RotateRel(float angle)
    {
		accumTime = 0.0f;
        hullState = TankHullState.Rotating;

		float startAngle = normalizeAngle(transform.localRotation.eulerAngles.y);
		moveVars.endAngleQ = Quaternion.Euler(0.0f, normalizeAngle(startAngle + angle), 0.0f);
		moveVars.rotDirection = (angle > 0.0f ? 1.0f : -1.0f);
		moveVars.rotationTotalTime = (angle / 360.0f);
    }	

    /// <summary>
	/// Rotate all the tank, to the specified angle (in degrees)
    /// </summary>
    public void RotateAbs(float angle)
    {
		accumTime = 0.0f;
		hullState = TankHullState.Rotating;

		float startAngle = transform.eulerAngles.y % 360;
		float endAngle = angle % 360;

		Debug.LogFormat("1 {0} {1}", startAngle, endAngle);

		if (Mathf.Abs(startAngle - endAngle) > 180.0f)
		{
			endAngle -= 360.0f;
		}

		moveVars.endAngleQ = Quaternion.Euler(0.0f, endAngle, 0.0f);
		moveVars.rotDirection = (endAngle > startAngle ? 1.0f : -1.0f);
		moveVars.rotationTotalTime = (Mathf.Abs(startAngle - endAngle) / 360.0f);

		Debug.LogFormat("2 {0} {1}", startAngle, endAngle);
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
        RotateAbs(45.0f);
    }
    
    public void DebugMove3()
    {
		float startAngle = normalizeAngle(0);
		float endAngle = normalizeAngle(startAngle + 350);

		float angleDif = Mathf.Abs(endAngle - startAngle);
		float totalTime = (angleDif / 360.0f) / hullRotationSpeed;

		Debug.LogFormat("s: {0} e: {1} d: {2} t: {3}", startAngle, endAngle, angleDif, totalTime);
    }

    /// <summary>
	/// Unity OnTriggerEnter method. Called when the tank hits a trigger in the scene.
	/// The reaction will be stop the tank immediatelly (and notify somehow?)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        hullState = TankHullState.Idle;
    }

    /// <summary>
    /// Returns an angle that is between (-360, 360). I will call it "normalized".
    /// </summary>
    /// <returns>The angle normalized</returns>
    /// <param name="angle">Angle to normalize</param>
    float normalizeAngle(float angle)
    {
    	float na = angle % 360;
    	return (na >= 0 ? na : 360 + na);
    }
}
