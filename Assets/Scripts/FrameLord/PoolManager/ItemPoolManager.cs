// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
	// The name of the pool
	public string poolName;

	// Reference to the item prefab
	public GameObject itemPrefab;

	// Min number of items to create
	public int minNumOfItems = 10;

	// List of idle items
	Queue<GameObject> listOfIdleItems;

	// List of used items
	List<GameObject> listOfUsedItems;

	// Items that should be recycled in the future
	List<ItemRecycleTime> listOfItemsToRecycle;

	// Unity Start Method
	void Start()
	{
		listOfIdleItems = new Queue<GameObject>();
		listOfUsedItems = new List<GameObject>();

		for (int i=0; i<minNumOfItems; i++)
		{
			GameObject newItem = GameObject.Instantiate(itemPrefab);
			newItem.SetActive(false);
			newItem.name = string.Format("{0}-{1}", itemPrefab.name, i);

			newItem.transform.parent = transform;

			listOfIdleItems.Enqueue(newItem);
		}

		listOfItemsToRecycle = new List<ItemRecycleTime>();
	}

	// Unity Update Method
	void Update()
	{
		if (listOfItemsToRecycle.Count > 0)
		{
			List<ItemRecycleTime> listToRemove = null;

			foreach (ItemRecycleTime itemR in listOfItemsToRecycle)
			{
				itemR.recycleTime -= Time.deltaTime;

				if (itemR.recycleTime <= 0.0f)
				{
					if (listToRemove == null) listToRemove = new List<ItemRecycleTime>();

					listToRemove.Add(itemR);

					RecycleItem(itemR.item);
				}
			}

			if (listToRemove != null)
			{
				for (int i=0; i<listToRemove.Count; i++)
				{
					listOfItemsToRecycle.Remove(listToRemove[i]);
				}
			}
		}
	}

	// Get a not used item
	public GameObject GetItem()
	{
		if (listOfIdleItems.Count > 0)
		{
			GameObject item = listOfIdleItems.Dequeue();

			Debug.AssertFormat(item != null, "ItemPoolManager@GetItem. item is null");

			listOfUsedItems.Add(item);

			IItem ii = item.GetComponent<IItem>();
			if (ii != null) ii.OnSpawned();

			return item;
		}
		else
		{
			Debug.LogWarning("MissileManager@GetItem. No more items in the pool.");
			return null;
		}
	}

	// Recycle an item
	public void RecycleItem(GameObject item)
	{
		item.SetActive(false);

		if (listOfUsedItems.Contains(item))
		{
			listOfUsedItems.Remove(item);
			listOfIdleItems.Enqueue(item);

			IItem ii = item.GetComponent<IItem>();
			if (ii != null) ii.OnDespawned();
		}
	}

	// Recycle an item after the specified amount of seconds
	public void RecycleItemAfterSecs(GameObject item, float delayInSecs)
	{
		listOfItemsToRecycle.Add(new ItemRecycleTime(item, delayInSecs));
	}
}
