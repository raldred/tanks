// Mono Framework
using System;

// Unity Framework
using UnityEngine;

public class SMAudioSourceGlobal : SMAudioSource
{
	public SndId sndId;

	protected override int getSndIdInt()
	{
		return (int) sndId;
	}
}
