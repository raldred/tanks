// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class Tree : MonoBehaviour, IMissileReceptor
{
	/// <summary>
    /// Reference to the explosion manager
    /// </summary>
    ItemPoolManager explosionManager;

	/// <summary>
	/// Unity Start Method
    /// </summary>
    void Start()
    {
		explosionManager = PoolManager.Instance.GetItemPoolManager("ExplosionManager");
    }

	/// <summary>
	/// Direct hit
	/// </summary>
	public void DirectHit(float missileHitDamage, Vector3 hitPosition)
	{
		GameObject explosion = explosionManager.GetItem();
		explosion.transform.position = hitPosition;
		explosion.SetActive(true);
		explosion.GetComponent<ParticleSystem>().Play();

		SoundManager.Instance.PlaySound(SndId.SND_EXPLOSION_OBSTACLE);
	}


	/// <summary>
	/// Area hit
	/// </summary>
	public void AreaHit(float missileHitDamage, Vector3 hitPosition)
	{

	}
	
}
