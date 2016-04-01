// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public delegate void OnMissileHit(Missile missile, GameObject hittedGameObject);

public class Missile : MonoBehaviour, IItem
{
	// Layer mask
    public LayerMask layerMask;

    public float missileDamage = 10.0f;

    // Detonation distance
    public float detonationDistance = 15f;

    // Missile life time
    public float lifeTime = 5f;

	// Delay despawn in ms
    public float despawnDelay;

	// Missile velocity
    public float velocity = 10f;

	// Missile despawn flag
    public bool DelayDespawn = false;

	// Array of delayed particles
    public ParticleSystem[] delayedParticles;

	// Array of Missile particles
    ParticleSystem[] particles;

	// Cached transform
    new Transform transform;

	// Missile hit flag
    bool isHit = false;

	// Missile timer
    float timer = 0f;

    // Missile flying direction
    Vector3 missileDir;

    // Reference to the mesh renderer
    MeshRenderer meshRenderer;

    // Reference to the missile manager
    ItemPoolManager missileManager;

    // Reference to the function that will be called when the missile hit something
	OnMissileHit onMissileHitFn;

	// Which game object was hitted by the missile
	GameObject hittedGameObject;

    // Unity Awake Method
    void Awake()
    {
        // Cache transform and get all particle systems attached
        transform = GetComponent<Transform>();
        particles = GetComponentsInChildren<ParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

	// Unity Start Method
    void Start()
    {
		missileManager = PoolManager.Instance.GetItemPoolManager("MissileManager");
    }

    // Unity Update Method
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

    // IItem.OnSpawned implementation
	// OnSpawned called by pool manager 
    public void OnSpawned()
    {       
        isHit = false;
        timer = 0f;
        missileDir = Vector3.zero;
        meshRenderer.enabled = true;
		hittedGameObject = null;
    }

	// IItem.OnDespawned implementation
	// OnDespawned called by pool manager when the missile is being recycled.
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
