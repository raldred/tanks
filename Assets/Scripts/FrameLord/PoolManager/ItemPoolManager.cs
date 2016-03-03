// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
	/// <summary>
	/// The name of the pool
	/// </summary>
	public string poolName;

	/// <summary>
	/// Reference to the item prefab
	/// </summary>
	public GameObject itemPrefab;

	/// <summary>
	/// Min number of items to create
	/// </summary>
	public int minNumOfItems = 10;

	/// <summary>
	/// List of idle items
	/// </summary>
	Queue<GameObject> listOfIdleItems;

	/// <summary>
	/// List of used items
	/// </summary>
	List<GameObject> listOfUsedItems;

	/// <summary>
	/// Unity Start Method
	/// </summary>
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
	}

	/// <summary>
	/// Get a not used item
	/// </summary>
	public GameObject GetItem()
	{
		if (listOfIdleItems.Count > 0)
		{
			GameObject item = listOfIdleItems.Dequeue();
			listOfUsedItems.Add(item);

			item.GetComponent<IItem>().OnSpawned();

			return item;
		}
		else
		{
			Debug.LogWarning("MissileManager@GetItem. No more items in the pool.");
			return null;
		}
	}

	/// <summary>
	/// Recycle an item
	/// </summary>
	public void RecycleItem(GameObject item)
	{
		item.SetActive(false);

		if (listOfUsedItems.Contains(item))
		{
			listOfUsedItems.Remove(item);
			listOfIdleItems.Enqueue(item);

			item.GetComponent<IItem>().OnDespawned();
		}
	}
}
