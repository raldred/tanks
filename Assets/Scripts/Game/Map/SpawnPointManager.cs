// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class SpawnPointManager : MonoBehaviorSingleton<SpawnPointManager>
{
	/// <summary>
	/// Array of spawn points
	/// </summary>
	SpawnPoint[] spawnPoints;

	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		spawnPoints = GetComponentsInChildren<SpawnPoint>();
	}

	/// <summary>
	/// Unity Update Method
	/// </summary>
	void Update()
	{
	
	}

	/// <summary>
	/// Returns the number of spawn point in the map
	/// </summary>
	public int GetSpawnPointCount()
	{
		return spawnPoints.Length;
	}

	/// <summary>
	/// Returns the position of the specified spawnpoint
	/// </summary>
	public KeyValuePair<int, int> GetSpawnPointPos(int num)
	{
		if (num >= 0 && num < spawnPoints.Length)
		{
			return Map.Instance.WorldPosToRowCol(spawnPoints[num].transform.position);
		}
		else
			return new KeyValuePair<int, int>(0, 0);
	}
}
