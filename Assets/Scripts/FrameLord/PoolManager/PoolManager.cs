// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class PoolManager : MonoBehaviorSingleton<PoolManager>
{
	/// <summary>
	/// Dictionary of pools
	/// </summary>
	Dictionary<string, ItemPoolManager> pools;

	/// <summary>
	/// Unity Awake Method
	/// </summary>
	void Awake()
	{
		pools = new Dictionary<string, ItemPoolManager>();

		ItemPoolManager[] itemPools = GetComponents<ItemPoolManager>();

		for (int i=0; i<itemPools.Length; i++)
		{
			if (!pools.ContainsKey(name))
				pools.Add(itemPools[i].poolName, itemPools[i]);
			else
				Debug.LogWarning("PoolManager@Awake. There are two item pool manangers with the same name. Duplicated items are not allowed.");
		}
	}

	/// <summary>
	/// Returns the reference to the specified item pool manager
	/// </summary>
	public ItemPoolManager GetItemPoolManager(string name)
	{
		if (pools.ContainsKey(name))
			return pools[name];
		else
			return null;
	}
}
