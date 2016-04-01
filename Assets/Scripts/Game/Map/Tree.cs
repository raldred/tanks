// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public class Tree : MapObstacle, IMissileReceptor
{
    // Reference to the explosion manager
    ItemPoolManager explosionManager;

    // Reference to the pool manager
    ItemPoolManager flameManager;

    float destroyTimer;

	// Unity Start Method
    new void Start()
    {
        base.Start();

        if (Application.isPlaying)
        {
			explosionManager = PoolManager.Instance.GetItemPoolManager("ExplosionManager");

			flameManager = PoolManager.Instance.GetItemPoolManager("FlameManager");
		}
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
		GameObject explosionItem = explosionManager.GetItem();
		explosionItem.transform.position = hitPosition;
		explosionItem.SetActive(true);
		explosionItem.GetComponent<ParticleSystem>().Play();
		explosionManager.RecycleItemAfterSecs(explosionItem, 3.0f);

		SoundManager.Instance.PlaySound(SndId.SND_EXPLOSION_OBSTACLE);

		health = Mathf.Clamp(health - missileHitDamage, 0.0f, float.MaxValue);

		if (health == 0.0f)
		{
			GameObject flameItem = flameManager.GetItem();
			flameItem.transform.position = new Vector3(transform.position.x, 1.7f, transform.position.z);
			flameItem.SetActive(true);
			flameItem.GetComponent<ParticleSystem>().Play();
			flameManager.RecycleItemAfterSecs(flameItem, 3.0f);

			// Destroy the object in 3 seconds
			destroyTimer = 3.0f;
		}

	}

	// Area hit
	public void AreaHit(float missileHitDamage, Vector3 hitPosition)
	{
    }
	
}
