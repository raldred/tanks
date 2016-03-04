// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public delegate void OnMissileHit(Missile missile, GameObject hittedGameObject);

public class Missile : MonoBehaviour, IItem
{
	/// <summary>
	/// Layer mask
	/// </summary>
    public LayerMask layerMask;

    public float missileDamage = 10.0f;

    /// <summary>
    /// Detonation distance
    /// </summary>
    public float detonationDistance = 15f;

    /// <summary>
    /// Missile life time
    /// </summary>
    public float lifeTime = 5f;

    /// <summary>
	/// Delay despawn in ms
    /// </summary>
    public float despawnDelay;

    /// <summary>
	/// Missile velocity
    /// </summary>
    public float velocity = 10f;

    /// <summary>
	/// Missile despawn flag
    /// </summary>
    public bool DelayDespawn = false;

    /// <summary>
	/// Array of delayed particles
    /// </summary>
    public ParticleSystem[] delayedParticles;

    /// <summary>
	/// Array of Missile particles
    /// </summary>
    ParticleSystem[] particles;

    /// <summary>
	/// Cached transform
    /// </summary>
    new Transform transform;

    /// <summary>
	/// Missile hit flag
    /// </summary>
    bool isHit = false;

    /// <summary>
	/// Missile timer
    /// </summary>
    float timer = 0f;

    /// <summary>
    /// Missile flying direction
    /// </summary>
    Vector3 missileDir;

    /// <summary>
    /// Reference to the mesh renderer
    /// </summary>
    MeshRenderer meshRenderer;

    /// <summary>
    /// Reference to the missile manager
    /// </summary>
    ItemPoolManager missileManager;

    /// <summary>
    /// Reference to the function that will be called when the missile hit something
    /// </summary>
	OnMissileHit onMissileHitFn;

	/// <summary>
	/// Which game object was hitted by the missile
	/// </summary>
	GameObject hittedGameObject;

    /// <summary>
    /// Unity Awake Method
    /// </summary>
    void Awake()
    {
        // Cache transform and get all particle systems attached
        transform = GetComponent<Transform>();
        particles = GetComponentsInChildren<ParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

	/// <summary>
	/// Unity Start Method
    /// </summary>
    void Start()
    {
		missileManager = PoolManager.Instance.GetItemPoolManager("MissileManager");
    }

	/// <summary>
    /// Unity Update Method
    /// </summary>
    void Update()
    {       
        // If something was hit
        if (isHit)
        {
            // Despawn current missile 
            if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                MissileDestroy();
        }
        // No collision occurred yet
        else
        {
            // Missile step per frame based on velocity and time
            missileDir = transform.forward * Time.deltaTime * velocity;
            RaycastHit hit;

			if (Physics.Raycast(transform.position, transform.forward, out hit, Vector3.Distance(transform.position, transform.position + missileDir), layerMask))
            {
				hittedGameObject = hit.transform.gameObject;
				IMissileReceptor imr = hittedGameObject.GetComponent<IMissileReceptor>();
				if (imr != null) imr.DirectHit(missileDamage, hit.point);

                HideMissile();

				if (onMissileHitFn != null) onMissileHitFn(this, hittedGameObject);
            }
            else
            {
                // Despawn missile at the end of life cycle
                if (timer >= lifeTime)
                {
                	Debug.Log("Missile dead by timeout");
                    HideMissile();
					MissileDestroy();
                }
            }

            // Advances missile forward
            transform.position += missileDir;
        }

        // Updates missile timer
        timer += Time.deltaTime;
    }

    /// <summary>
	/// IItem.OnSpawned implementation
	/// OnSpawned called by pool manager 
    /// </summary>
    public void OnSpawned()
    {       
        isHit = false;
        timer = 0f;
        missileDir = Vector3.zero;
        meshRenderer.enabled = true;
		hittedGameObject = null;
    }

	/// <summary>
	/// IItem.OnDespawned implementation
	/// OnDespawned called by pool manager when the missile is being recycled.
    /// </summary>
    public void OnDespawned()
    {          
    }

    // Stop attached particle systems emission and allow them to fade out before despawning
    void Delay()
    {       
        if (particles.Length > 0 && delayedParticles.Length > 0)
        {
            bool delayed;

            for (int i = 0; i < particles.Length; i++)
            {
                delayed = false;

                for (int y = 0; y < delayedParticles.Length; y++)             
                    if (particles[i] == delayedParticles[y])
                    {
                        delayed = true;
                        break;
                    }                

                particles[i].Stop(false);

                if (!delayed)
                    particles[i].Clear(false);                
            }
        }
    }

    // Despawn routine
    void MissileDestroy()
    {   
		missileManager.RecycleItem(this.gameObject);
    }
    
    void HideMissile()
    {
        meshRenderer.enabled = false;
        isHit = true;

        // Invoke delay routine if required
        if (DelayDespawn)
        {
            // Reset missile timer and let particles systems stop emitting and fade out correctly
            timer = 0f;
            Delay();
        }
    }

	public void SetOnMissileHit(OnMissileHit onHitFn)
	{
		onMissileHitFn = onHitFn;
	}
}
