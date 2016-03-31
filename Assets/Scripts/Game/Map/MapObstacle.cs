// Unity Framework
using UnityEngine;

[ExecuteInEditMode]
public class MapObstacle : MonoBehaviour
{
	// Snap position in the editor
	public bool snapPosition = true;

    // Is the obstacle breakable?
    public bool isBreakable = true;
    
    // Obstacle health (make sense if the obstacle is breakable)
    public float health = 5.0f;

    // Y position offset value
    public float yPos = 0.0f;

    // Row positive extension
    public float rowPosExtension = 0;

	// Row negative extension
	public float rowNegExtension = 0;

	// Col positive extension
	public float colPosExtension = 0;

	// Col negative extension
	public float colNegExtension = 0;


    // Cell where the obstacle is related
	protected MapCell cell;

    // Unity Start Method
    protected void Start()
    {
		if (snapPosition) doSnapPosition();
    }

	// Unity Update Method
    protected void Update()
    {
    	// The position will be snapped to the cells
		if (snapPosition && !Application.isPlaying)
		{
			doSnapPosition();
		}
    }

    // Snap the position of the object
	void doSnapPosition()
    {
		float x = (Mathf.Floor(transform.position.x / Map.Instance.cellSize) * Map.Instance.cellSize);
		float y = yPos;
		float z = (Mathf.Floor(transform.position.z / Map.Instance.cellSize) * Map.Instance.cellSize);
		transform.position = new Vector3(x, y, z);
    }

    // Relate the obstacle with the cell
    public void SetCell(MapCell cell)
    {
    	this.cell = cell;
    }
}
