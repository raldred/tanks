// Unity Engine
using UnityEngine;

// Vectrosity
using Vectrosity;

public class Tank : MonoBehaviour
{
    public Color debugLineColor = Color.blue;
    
    // Hull Rotation Speed
    float hullRotationSpeed = 1.0f;
    
    // Speed Movement
    float speedMovement = 1.0f;
    
    
    // State of the hull of the tank
    TankHullState hullState;
    
    // Time accumulator
    float accumTime;
    
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
    
    MoveVars moveVars = new MoveVars();
    
    VectorLine vl;
    
    // Turret
    TankTurret turret;
    
	// Unity Start Method
	void Start()
	{
       hullState = TankHullState.Idle;
       
       turret = new TankTurret();
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
	}

	// Rotate the turret clockwise using a relative angle
	public void RotateTurretRel(float angle)
	{

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
        //Map.Instance.WorldPosToRowCol
        
        vl = VectorLine.SetLine3D(debugLineColor, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), new Vector3(moveVars.targetPos.x, moveVars.targetPos.y + 0.1f, moveVars.targetPos.z));
        vl.SetWidth(2.0f);
        vl.Draw3D();
        
	}

	/// <summary>
	/// Move the tank at full speed to the specified row/col map position. Returns false
	/// if it's not possible to move the tank to that position.
	/// </summary>
	public bool MoveToRowCol(int row, int col)
	{
		return false;
	}
    
    public void DebugMove1()
    {
        MoveToWorldPos(new Vector3(35f, 0f, 35f));
    }
    

}
