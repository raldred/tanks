// Unity Framework
using UnityEngine;

class ItemRecycleTime
{
	// Item to recycle
	public GameObject item;

	// Time seconds to recycle the item
	public float recycleTime;

	// Constructor
	public ItemRecycleTime(GameObject it, float rt)
	{
		item = it;
		recycleTime = rt;
	}
}