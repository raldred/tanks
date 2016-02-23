using UnityEngine;
using System.Collections;

using Vectrosity;

[ExecuteInEditMode]
public class Map : MonoBehaviorSingleton<Map>
{
	/// <summary>
	/// Number of rows of the map
	/// </summary>
	public int Rows = 15;

	/// <summary>
	/// Number of cols of the map
	/// </summary>
	public int Cols = 15;

	/// <summary>
	/// Cell size
	/// </summary>
	public float cellSize = 3.0f;

	/// <summary>
	/// Map data structure
	/// </summary>
	public MapCell[,] MapData;

	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		CreateMapData();
	}

	void OnDrawGizmos()
	{
		DrawGrid();
	}

    // Draw the debug grid lines
	void DrawGrid()
	{
		if (MapData == null) return;

		float cellSizeDiv2 = cellSize / 2.0f;
		for (int row=0; row<Rows; row++)
		{
			for (int col=0; col<Cols; col++)
			{
				Vector3 pnt1 = new Vector3(row * cellSize - cellSizeDiv2, 0.0f, col * cellSize - cellSizeDiv2);
				Vector3 pnt2 = new Vector3(row * cellSize + cellSizeDiv2, 0.0f, col * cellSize - cellSizeDiv2);
				Vector3 pnt3 = new Vector3(row * cellSize + cellSizeDiv2, 0.0f, col * cellSize + cellSizeDiv2);
				Vector3 pnt4 = new Vector3(row * cellSize - cellSizeDiv2, 0.0f, col * cellSize + cellSizeDiv2);

				Color clr = Color.red;
				if (MapData[row, col].CellType == MapCellType.Breakable) clr = Color.blue;
				else if (MapData[row, col].CellType == MapCellType.Unbreakable) clr = Color.cyan;

				Debug.DrawLine(pnt1, pnt2, clr);
				Debug.DrawLine(pnt2, pnt3, clr);
				Debug.DrawLine(pnt3, pnt4, clr);
				Debug.DrawLine(pnt4, pnt1, clr);
			}
		}
	}

	void CreateMapData()
	{
		MapData = new MapCell[Rows,Cols];

		for (int row=0; row<Rows; row++)
		{
			for (int col=0; col<Cols; col++)
			{
				MapData[row, col].CellType = MapCellType.None;
				MapData[row, col].Row = row;
				MapData[row, col].Col = col;
			}
		}
	}

	public bool RowColToWorldPos(int row, int col, out Vector3 pos)
	{
		pos = Vector3.zero;

		return true;
	}

    // Convert from world position to map position
	public bool WorldPosToRowCol(Vector3 pos, out int row, out int col)
	{
		col = 0;
		row = 0;
		return true;
	}

    // Check the map position
    public bool CheckPos(int row, int col)
    {
        bool check = (row >= 0 && row < Rows && col >= 0 && col < Cols);
        Debug.AssertFormat(check, "Map.CheckPos failed. Row / Col: {0} / {1} is not valid.", row, col);
        return check;
    }
}
