using UnityEngine;
using System.Collections;

public class TankAI : MonoBehaviour
{


	/// <summary>
	/// Tank init
	/// </summary>
	public virtual void Init() {}

	/// <summary>
	/// Called one time per Update
	/// </summary>
	public virtual void Think() {}

	/// <summary>
	/// Shoot
	/// </summary>
	public virtual void OnShoot() {}

	/// <summary>
	/// Shoot hits something
	/// </summary>
	public virtual void OnShootHit(GameObject hittedGameObject) {}
}
