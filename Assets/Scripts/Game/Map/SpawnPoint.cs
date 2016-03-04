// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
	/// <summary>
	/// Unity Start Method
	/// </summary>
	void Start()
	{
		if (Application.isPlaying)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
	}

	/// <summary>
	/// Unity Update Method
	/// </summary>
	void Update()
	{
		if (!Application.isPlaying)
		{
			float x = (Mathf.Floor(transform.position.x / Map.Instance.cellSize) * Map.Instance.cellSize);// + (Map.Instance.cellSize / 2.0f);
			float y = 0.75f;
			float z = (Mathf.Floor(transform.position.z / Map.Instance.cellSize) * Map.Instance.cellSize);// + (Map.Instance.cellSize / 2.0f);

			transform.position = new Vector3(x, y, z);
		}
	}
}
