using UnityEngine;
using System.Collections;

public interface TankAI
{
	/// <summary>
	/// Tank init
	/// </summary>
	void Init();

	/// <summary>
	/// Called one time per Update
	/// </summary>
	void Think();
}
