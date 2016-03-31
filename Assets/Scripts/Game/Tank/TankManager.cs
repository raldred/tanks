// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class TankManager : MonoBehaviorSingleton<TankManager>
{
	// Prefab of the tank
	public GameObject tankPrefab;

	// Tank material flavors
	public Material[] tankMaterial;

	// Tank AIs
	public GameObject[] tankAI;

	// Dictionary of tanks
	Dictionary<string, Tank> tankDic;

	// Unity Start Method
	void Start()
	{
		tankDic = new Dictionary<string, Tank>();

		SpawnTanks();
	}

	// Will look for the spawn points and instantiate the tanks
	public void SpawnTanks()
	{
        tankDic.Clear();
        
        int spc = SpawnPointManager.Instance.GetSpawnPointCount();
        
        for (int i=0; i<spc; i++)
        {
            KeyValuePair<int, int> spawnPoint = SpawnPointManager.Instance.GetSpawnPointPos(i);
            spawnTank(spawnPoint, SpawnPointManager.Instance.GetSpawnPointFaceDir(i), i);        
        }
		
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
        
        // Change the material to the tank
        tank.SetMaterial(tankMaterial[num]);

		tankDic.Add(tankGO.name, tank);
	}

	// Returns tank by name
	public Tank GetTankByName(string tankName)
	{
		return tankDic[tankName];
	}
    
    // Return all the tanks in the field. This method should not be called
    // by the Ais.
    public List<Tank> GetTanks()
    {
        List<Tank> tanks = new List<Tank>();
        foreach (KeyValuePair<string, Tank> tank in tankDic) tanks.Add(tank.Value);
        return tanks;
    }

	public void DebugCommand1()
	{
		//tankDic["Tank-0"].Shoot(new Vector3(1.0f, 0.0f, 1.0f));
        tankDic["Tank-0"].GetViewInfo();
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
