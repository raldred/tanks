using UnityEngine;
using System.Collections;

public class Angle
{
	/// <summary>
    /// Returns an angle that is between (-360, 360). I will call it "normalized".
    /// </summary>
    public static float Normalize(float angle)
    {
    	float na = angle % 360;
    	return (na >= 0 ? na : 360 + na);
    }

    /// <summary>
    /// Returns the shortest path to go from angle1 to angle2
    /// </summary>
    public static float GetClosestAngleDif(float angle1, float angle2)
    {
		float dif = angle2 - angle1;
		float dif1 = ((dif + 180.0f) % 360.0f);
		return (dif1 > 0.0f ? dif1 : dif1 + 360.0f) - 180.0f;
    }
}
