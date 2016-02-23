using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour
{

	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
	
	}
	
	/// <summary>
	/// Unity Update Method
	/// </summary>
	void Update()
	{
	
	}

	/// <summary>
	/// Rotate the tower clockwise using a relative angle
	/// </summary>
	public void RotateTurretRel(float angle)
	{

	}

	/// <summary>
	/// Rotate the tower clockwise using an absolute angle
	/// </summary>
	public void RotateTurretAbs(float angle)
	{
		
	}

	/// <summary>
	/// Move the tank at full speed to the specified world position
	/// </summary>
	public void MoveToWorldPos(Vector2 pos)
	{

	}

	/// <summary>
	/// Move the tank at full speed to the specified row/col map position. Returns false
	/// if it's not possible to move the tank to that position.
	/// </summary>
	public bool MoveToRowCol(int row, int col)
	{
		return false;
	}
}
