using UnityEngine;
using System.Collections.Generic;

using Vectrosity;

[ExecuteInEditMode]
public class Map : MonoBehaviorSingleton<Map>
{
    const float GridLineWidth = 2.0f;
    
	// Number of rows of the map
	public int Rows = 15;

	// Number of cols of the map
	public int Cols = 15;

	// Cell size
	public float cellSize = 3.0f;

	// Map data structure
	public MapCell[,] MapData;

    VectorLine[] gridLines;

    public bool gridLineVisible = true;

    public bool GridLineVisible
    {
        get { return gridLineVisible; }
        set { 
            gridLineVisible = value;
            for (int i=0; i<gridLines.Length; i++)
            {
                gridLines[i].SetWidth(0.0f);
            }
        }
    }
    
    // Called from the UI. Change the visible state of the debug lines
    public void GridLineVisibleChange()
    {
        gridLineVisible = !gridLineVisible;
        for (int i=0; i<gridLines.Length; i++) gridLines[i].SetWidth(gridLineVisible ? GridLineWidth : 0.0f);
    }

	// Unity Start Method
	void Start()
	{
		CreateMapData();
        
        if (Application.isPlaying)
        {
            DrawGridV();
        
            PopulateMapInfoWithObstacles();
            
            DrawGridObstacles();
        }
	}
    
    // Unity OnDrawGizmos Method
    void OnDrawGizmos()
    {
        DrawGrid();
    }

    // Draw the debug grid lines (visible only in the editor window)
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
    
    // Draw the map grid line
    void DrawGridV()
    {
        if (MapData == null) return;
     
        Vector3 v1, v2;
     
        gridLines = new VectorLine[Rows + Cols + 2];
        int idx = 0;
        
        float cellSizeDiv2 = cellSize / 2.0f;
		for (int row=0; row<Rows; row++)
		{
            v1 = new Vector3(row * cellSize - cellSizeDiv2, 0.1f, -cellSizeDiv2);
            v2 = new Vector3(row * cellSize - cellSizeDiv2, 0.1f, Cols * cellSize - cellSizeDiv2);
            
            gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
            gridLines[idx - 1].SetWidth(GridLineWidth);
        }
        v1 = new Vector3(Rows * cellSize - cellSizeDiv2, 0.1f, -cellSizeDiv2);
        v2 = new Vector3(Rows * cellSize - cellSizeDiv2, 0.1f, Cols * cellSize - cellSizeDiv2);
        gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
        gridLines[idx - 1].SetWidth(GridLineWidth);
        gridLines[idx - 1].Draw();
            
		for (int col=0; col<Cols; col++)
		{
            v1 = new Vector3(-cellSizeDiv2, 0.1f, col * cellSize - cellSizeDiv2);
            v2 = new Vector3(Rows * cellSize - cellSizeDiv2, 0.1f, col * cellSize - cellSizeDiv2);
            
            gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
            gridLines[idx - 1].SetWidth(GridLineWidth);
        }
        v1 = new Vector3(-cellSizeDiv2, 0.1f, Cols * cellSize - cellSizeDiv2);
        v2 = new Vector3(Rows * cellSize - cellSizeDiv2, 0.1f, Cols * cellSize - cellSizeDiv2);
        gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
        gridLines[idx - 1].SetWidth(GridLineWidth);
        
        if (gridLineVisible)
        {
            for (int i=0; i<gridLines.Length; i++)
            {
                gridLines[i].Draw3D();
            }
        }
        
    }
    
    List<VectorLine> gridObstaclesLines;
    void DrawGridObstacles()
    {
        if (gridObstaclesLines != null)
        {
            for (int i=0; i<gridObstaclesLines.Count; i++)
            {
                VectorLine vl = gridObstaclesLines[i];
                VectorLine.Destroy(ref vl);    
            }
        }
        
        gridObstaclesLines = new List<VectorLine>();
        
        for (int row=0; row<Rows; row++)
		{
            for (int col=0; col<Cols; col++)
            {
                if (MapData[row, col].CellType == MapCellType.Breakable || MapData[row, col].CellType == MapCellType.Unbreakable)
                {
                    VectorLine vlo = new VectorLine("obs", new List<Vector3>(5), null, 2.0f, LineType.Continuous);
                    vlo.SetColor(MapData[row, col].CellType == MapCellType.Breakable ? Color.blue : Color.cyan);
                    vlo.MakeRect(MapData[row, col].Pos + new Vector3(cellSize / -2.0f, 1.0f, cellSize / -2.0f),
                                 MapData[row, col].Pos + new Vector3(cellSize / 2.0f, 1.0f, cellSize / 2.0f));
                                 
                    gridObstaclesLines.Add(vlo);
                }
            }
        }
        
        for (int i=0; i<gridObstaclesLines.Count; i++)
        {
            gridObstaclesLines[i].Draw3D();    
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
                MapData[row, col].Pos = RowColToWorldPos(row, col);
			}
		}
	}

	public Vector3 RowColToWorldPos(int row, int col)
	{
        return new Vector3(row * cellSize, 0.0f, col * cellSize);
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
    
    // Update the MapData structure with the celltype information
    // The function checks only for breakable and unbreakable obstacles (not for tanks or anything else)
    void PopulateMapInfoWithObstacles()
    {
        Vector3 outPos;
        RaycastHit hit;
        
        for (int row=0; row<Rows; row++)
		{
			for (int col=0; col<Cols; col++)
			{
                outPos = RowColToWorldPos(row, col);
				
                if (Physics.Raycast(outPos + Vector3.up * 5.0f, Vector2.down, out hit, 100.0f, Layer.Breakable | Layer.Unbreakable))
                {
                    if (hit.transform.gameObject.layer == Layer.BreakableNum)
                    {
                        MapData[row, col].CellType = MapCellType.Breakable;
                    }
                    else if (hit.transform.gameObject.layer == Layer.UnbreakableNum)
                    {
                        MapData[row, col].CellType = MapCellType.Unbreakable;
                    }
                }
                else
                {
                    MapData[row, col].CellType = MapCellType.None;
                }
			}
		}
    }
    
    public void Test()
    {
        PopulateMapInfoWithObstacles();
        DrawGridObstacles();
    }
}
