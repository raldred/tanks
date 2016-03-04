// Mono Framework
using System.Collections.Generic;

// Unity Engine
using UnityEngine;

public interface IMissileReceptor
{
	/// <summary>
	/// Direct hit
	/// </summary>
	void DirectHit(float missileHitDamage, Vector3 hitPosition);

	/// <summary>
	/// Area hit
	/// </summary>
	void AreaHit(float missileHitDamage, Vector3 hitPosition);
}
