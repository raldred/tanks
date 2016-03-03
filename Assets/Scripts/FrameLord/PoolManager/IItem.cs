using UnityEngine;
using System.Collections;

public interface IItem
{
	/// <summary>
	/// Called when the item is spawned
	/// </summary>
	void OnSpawned();

	/// <summary>
	/// Called after the item is recycled
	/// </summary>
    void OnDespawned();
}
