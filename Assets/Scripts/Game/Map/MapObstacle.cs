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

    // Cell where the obstacle is related
	MapCell cell;

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
		float y = 0.0f;
		float z = (Mathf.Floor(transform.position.z / Map.Instance.cellSize) * Map.Instance.cellSize);
		transform.position = new Vector3(x, y, z);
    }

    // Relate the obstacle with the cell
    public void SetCell(MapCell cell)
    {
    	this.cell = cell;
    }
}
