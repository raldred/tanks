// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class Tree : MapObstacle, IMissileReceptor
{
    // Reference to the explosion manager
    ItemPoolManager explosionManager;

    float destroyTimer;

	// Unity Start Method
    new void Start()
    {
        base.Start();

        if (Application.isPlaying)
			explosionManager = PoolManager.Instance.GetItemPoolManager("ExplosionManager");
    }

    // Unity Update Method
    new void Update()
    {
    	base.Update();

		if (destroyTimer > 0.0f)
		{
			destroyTimer -= Time.deltaTime;

			if (destroyTimer <= 0.0f)
			{
				// Clear the reference in the cell
				cell.Obstacle = null;

				UnityEngine.Object.Destroy(this.gameObject);
			}
		}
    }

	// Direct hit
	public void DirectHit(float missileHitDamage, Vector3 hitPosition)
	{
		GameObject explosion = explosionManager.GetItem();
		explosion.transform.position = hitPosition;
		explosion.SetActive(true);
		explosion.GetComponent<ParticleSystem>().Play();

		SoundManager.Instance.PlaySound(SndId.SND_EXPLOSION_OBSTACLE);

		// Destroy the object in 3 seconds
		destroyTimer = 3.0f;
	}

	// Area hit
	public void AreaHit(float missileHitDamage, Vector3 hitPosition)
	{
    }
	
}
