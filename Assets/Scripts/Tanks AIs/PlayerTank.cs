using UnityEngine;
using System.Collections;

public class PlayerTank : TankAI
{
	/// <summary>
	/// Tank Init
	/// </summary>
	public override void Init()
	{
	
	}

	/// <summary>
	/// Tank Think
	/// </summary>
	public override void Think()
	{
	
	}

	public override void OnShoot()
	{
		Debug.Log("Tank.OnShoot");
	}

	public override void OnShootHit(GameObject hittedGameObject)
	{
		Debug.LogFormat("Tank.OnShootHit: {0}", hittedGameObject.name);
	}
}
