using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class F3DParticleScale : MonoBehaviour, IItem
{
	/// <summary>
	/// Particle scale
	/// </summary>
    [Range(0f, 20f)]
    public float ParticleScale = 1.0f;

    /// <summary>
	/// Should the game be scaled as well
    /// </summary>
    public bool ScaleGameobject = true;

    /// <summary>
	/// Previous scale
    /// </summary>
    float prevScale;

    /// <summary>
    /// Unity Start Method
    /// </summary>
    void Start()
    { 
        // Store previous scale
        prevScale = ParticleScale;
    }    

    /// <summary>
    /// Unity Update Method
    /// </summary>
	void Update()
    {
        #if UNITY_EDITOR
        if (prevScale != ParticleScale && ParticleScale > 0)
        {
            if (ScaleGameobject)
                transform.localScale =
                new Vector3(ParticleScale, ParticleScale, ParticleScale);

            float scaleFactor = ParticleScale / prevScale;

            ScaleShurikenSystems(scaleFactor);
            ScaleTrailRenderers(scaleFactor);

            prevScale = ParticleScale;
        }
        #endif
    }

	/// <summary>
	/// IItem.OnSpawned implementation
	/// OnSpawned called by pool manager 
    /// </summary>
    public void OnSpawned()
    {       
    }

	/// <summary>
	/// IItem.OnDespawned implementation
	/// OnDespawned called by pool manager when the missile is being recycled.
    /// </summary>
    public void OnDespawned()
    {          
    }

    /// <summary>
	/// Scale Shuriken particle system 
    /// </summary>
    void ScaleShurikenSystems(float scaleFactor)
    {
        #if UNITY_EDITOR
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem system in systems)
        {
            system.startSpeed *= scaleFactor;
            system.startSize *= scaleFactor;
            system.gravityModifier *= scaleFactor;

            SerializedObject so = new SerializedObject(system);

            so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
            so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
            so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;          

            so.ApplyModifiedProperties();
        }
        #endif
    }

    /// <summary>
	/// Scale trail renderer
    /// </summary>
    void ScaleTrailRenderers(float scaleFactor)
    {
        TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();

        foreach (TrailRenderer trail in trails)
        {
            trail.startWidth *= scaleFactor;
            trail.endWidth *= scaleFactor;
        }
    }


}