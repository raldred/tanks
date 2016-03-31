// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class Tree : MapObstacle, IMissileReceptor
{
    // Reference to the explosion manager
    ItemPoolManager explosionManager;

	// Unity Start Method
    new void Start()
    {
        base.Start();
		explosionManager = PoolManager.Instance.GetItemPoolManager("ExplosionManager");
    }

	// Direct hit
	public void DirectHit(float missileHitDamage, Vector3 hitPosition)
	{
		GameObject explosion = explosionManager.GetItem();
		explosion.transform.position = hitPosition;
		explosion.SetActive(true);
		explosion.GetComponent<ParticleSystem>().Play();

		SoundManager.Instance.PlaySound(SndId.SND_EXPLOSION_OBSTACLE);
	}

	// Area hit
	public void AreaHit(float missileHitDamage, Vector3 hitPosition)
	{
    }
	
}
