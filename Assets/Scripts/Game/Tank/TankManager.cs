// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class TankManager : MonoBehaviorSingleton<TankManager>
{
	/// <summary>
	/// Prefab of the tank
	/// </summary>
	public GameObject tankPrefab;

	/// <summary>
	/// Tank material flavors
	/// </summary>
	public Material[] tankMaterial;

	/// <summary>
	/// Tank AIs
	/// </summary>
	public GameObject[] tankAI;

	/// <summary>
	/// Dictionary of tanks
	/// </summary>
	Dictionary<string, Tank> tankDic;

	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		tankDic = new Dictionary<string, Tank>();

		SpawnTanks();
	}

	/// <summary>
	/// Unity Update Method
	/// </summary>
	void Update()
	{
	
	}

	/// <summary>
	/// Will look for the spawn points and instantiate the tanks
	/// </summary>
	public void SpawnTanks()
	{
		KeyValuePair<int, int> spawnPoint = SpawnPointManager.Instance.GetSpawnPointPos(0);

		tankDic.Clear();

		spawnTank(spawnPoint, new Vector3(0f, 0f, 1f), 0);
	}

	void spawnTank(KeyValuePair<int, int> spawnPoint, Vector3 faceDir, int num)
	{
		GameObject tankGO = GameObject.Instantiate(tankPrefab);

		tankGO.transform.position = Map.Instance.RowColToWorldPos(spawnPoint.Key, spawnPoint.Value);
		tankGO.name = string.Format("Tank-{0}", num);
		tankGO.transform.forward = new Vector3(faceDir.x, 0f, faceDir.z);

		TankAI ai = tankAI[num].GetComponent<TankAI>();
		Tank tank = tankGO.GetComponent<Tank>();
		tank.SetAI(ai);

		tankDic.Add(tankGO.name, tank);
	}

	/// <summary>
	/// Returns tank by name
	/// </summary>
	public Tank GetTankByName(string tankName)
	{
		return tankDic[tankName];
	}

	public void DebugCommand1()
	{
		tankDic["Tank-0"].Shoot(new Vector3(1.0f, 0.0f, 1.0f));
	}

	public void DebugCommand2()
	{
		tankDic["Tank-0"].Shoot(new Vector3(1.0f, 0.0f, 1.0f));
	}

	public void DebugCommand3()
	{
		tankDic["Tank-0"].Shoot(new Vector3(1.0f, 0.0f, 1.0f));
	}
}
