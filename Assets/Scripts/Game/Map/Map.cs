// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

// Vectrosity Framework
using Vectrosity;

[ExecuteInEditMode]
public class Map : MonoBehaviorSingleton<Map>
{
    const float gridLineWidth = 2.0f;
    
	// Number of rows of the map
	public int rows = 15;

	// Number of cols of the map
	public int cols = 15;

	// Cell size
	public float cellSize = 3.0f;

	// Map data structure
	public MapCell[,] mapData;

	// List of static obstacles (tanks are not included here). The obstacles
	// are static but some of them could be destroyed
	List<MapObstacle> obstacles;

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
        for (int i=0; i<gridLines.Length; i++) gridLines[i].SetWidth(gridLineVisible ? gridLineWidth : 0.0f);
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
		if (mapData == null) return;

		float cellSizeDiv2 = cellSize / 2.0f;
		for (int row=0; row<rows; row++)
		{
			for (int col=0; col<cols; col++)
			{
				Vector3 pnt1 = new Vector3(row * cellSize - cellSizeDiv2, 0.0f, col * cellSize - cellSizeDiv2);
				Vector3 pnt2 = new Vector3(row * cellSize + cellSizeDiv2, 0.0f, col * cellSize - cellSizeDiv2);
				Vector3 pnt3 = new Vector3(row * cellSize + cellSizeDiv2, 0.0f, col * cellSize + cellSizeDiv2);
				Vector3 pnt4 = new Vector3(row * cellSize - cellSizeDiv2, 0.0f, col * cellSize + cellSizeDiv2);

				Color clr = Color.red;
				if (mapData[row, col].Obstacle != null)
				{
					if (mapData[row, col].Obstacle.isBreakable) clr = Color.blue;
					else clr = Color.cyan;
				} 

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
        if (mapData == null) return;
     
        Vector3 v1, v2;
     
        gridLines = new VectorLine[rows + cols + 2];
        int idx = 0;
        
        float cellSizeDiv2 = cellSize / 2.0f;
		for (int row=0; row<rows; row++)
		{
            v1 = new Vector3(row * cellSize - cellSizeDiv2, 0.1f, -cellSizeDiv2);
            v2 = new Vector3(row * cellSize - cellSizeDiv2, 0.1f, cols * cellSize - cellSizeDiv2);
            
            gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
            gridLines[idx - 1].SetWidth(gridLineWidth);
        }
        v1 = new Vector3(rows * cellSize - cellSizeDiv2, 0.1f, -cellSizeDiv2);
        v2 = new Vector3(rows * cellSize - cellSizeDiv2, 0.1f, cols * cellSize - cellSizeDiv2);
        gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
        gridLines[idx - 1].SetWidth(gridLineWidth);
        gridLines[idx - 1].Draw();
            
		for (int col=0; col<cols; col++)
		{
            v1 = new Vector3(-cellSizeDiv2, 0.1f, col * cellSize - cellSizeDiv2);
            v2 = new Vector3(rows * cellSize - cellSizeDiv2, 0.1f, col * cellSize - cellSizeDiv2);
            
            gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
            gridLines[idx - 1].SetWidth(gridLineWidth);
        }
        v1 = new Vector3(-cellSizeDiv2, 0.1f, cols * cellSize - cellSizeDiv2);
        v2 = new Vector3(rows * cellSize - cellSizeDiv2, 0.1f, cols * cellSize - cellSizeDiv2);
        gridLines[idx++] = VectorLine.SetLine3D(Color.red, v1, v2);
        gridLines[idx - 1].SetWidth(gridLineWidth);
        
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
        
        for (int row=0; row<rows; row++)
		{
            for (int col=0; col<cols; col++)
            {
                if (mapData[row, col].Obstacle != null)
                {
                    VectorLine vlo = new VectorLine("obs", new List<Vector3>(5), null, 2.0f, LineType.Continuous);
					vlo.SetColor(mapData[row, col].Obstacle.isBreakable ? Color.blue : Color.cyan);
                    vlo.MakeRect(mapData[row, col].Pos + new Vector3(cellSize / -2.0f, 1.0f, cellSize / -2.0f),
                                 mapData[row, col].Pos + new Vector3(cellSize / 2.0f, 1.0f, cellSize / 2.0f));
                                 
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
		mapData = new MapCell[rows,cols];

		for (int row=0; row<rows; row++)
		{
			for (int col=0; col<cols; col++)
			{
				mapData[row, col].Row = row;
				mapData[row, col].Col = col;
                mapData[row, col].Pos = RowColToWorldPos(row, col);
			}
		}
	}

	public Vector3 RowColToWorldPos(int row, int col)
	{
        return new Vector3(row * cellSize, 0.0f, col * cellSize);
	}

    // Convert from world position to map position
	public KeyValuePair<int, int> WorldPosToRowCol(Vector3 pos)
	{
        int row = (int) (pos.x / cellSize);
        int col = (int) (pos.z / cellSize);
		return new KeyValuePair<int, int>(row, col);
	}

    // Check the map position
    public bool CheckPos(int row, int col)
    {
        bool check = (row >= 0 && row < rows && col >= 0 && col < cols);
        Debug.AssertFormat(check, "Map.CheckPos failed. Row / Col: {0} / {1} is not valid.", row, col);
        return check;
    }
    
	// Update the MapData structure with the celltype information
    // The function checks only for breakable and unbreakable obstacles (not for tanks or anything else)
    void PopulateMapInfoWithObstacles()
    {
		MapObstacle[] obs = transform.GetComponentsInChildren<MapObstacle>();

		for (int i=0; i<obs.Length; i++)
		{
			KeyValuePair<int, int> rowcol = WorldPosToRowCol(obs[i].transform.position);
			int row = (int) rowcol.Key;
			int col = (int) rowcol.Value;
			mapData[row, col].Obstacle = obs[i];
			obs[i].SetCell(mapData[row, col]);

			if (obs[i].rowPosExtension == 1 && row + 1 < rows) mapData[row + 1, col].Obstacle = obs[i];
			if (obs[i].rowNegExtension == 1 && row - 1 >= 0) mapData[row - 1, col].Obstacle = obs[i];
			if (obs[i].colPosExtension == 1 && col + 1 < cols) mapData[row, col + 1].Obstacle = obs[i];
			if (obs[i].colNegExtension == 1 && col - 1 >= 0) mapData[row, col - 1].Obstacle = obs[i];
			if (obs[i].rowPosExtension == 1 && row + 1 < rows && obs[i].colPosExtension == 1 && col + 1 < cols) mapData[row + 1, col + 1].Obstacle = obs[i];
			if (obs[i].rowNegExtension == 1 && row - 1 >= 0 && obs[i].colPosExtension == 1 && col + 1 < cols) mapData[row - 1, col + 1].Obstacle = obs[i];
			if (obs[i].rowPosExtension == 1 && row + 1 < rows && obs[i].colNegExtension == 1 && col - 1 >= 0) mapData[row + 1, col - 1].Obstacle = obs[i];
			if (obs[i].rowNegExtension == 1 && row - 1 >= 0 && obs[i].colNegExtension == 1 && col - 1 >= 0) mapData[row - 1, col - 1].Obstacle = obs[i];
		}
    } 

    // Returns all the obstacles in the map
	public MapObstacle[] GetObstacles()
    {
		return transform.GetComponentsInChildren<MapObstacle>();
    }
}
