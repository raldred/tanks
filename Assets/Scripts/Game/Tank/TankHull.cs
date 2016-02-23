using UnityEngine;
using System.Collections;

public class TankHull
{
    // State of the hull of the tank
    TankHullState state;
    
    // Time accumulator
    float accumTime;
    
	// Custom Start Method
	public void Start()
    {
	   state = TankHullState.Idle;
	}
	
	// Custom Update Method
	public void Update()
    {
	
	}
    
    public void RotateRel(float angle)
    {
        
    }
}
