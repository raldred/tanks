// Mono Framework
using System;
using System.Collections;
using System.Xml;
using System.IO;

// Unity Engine
using UnityEngine;


public enum SndId : int
{
	SND_NONE = 0,

	SND_TANK_ENGINE = 0,
	SND_TANK_TURRET_ROTATE,
	SND_TANK_SHOOT
}

/// <summary>
///
/// </summary>
public class SoundListGame : SoundList
{
	SoundProp[] sounds = {
		new SoundProp((int) SndId.SND_TANK_ENGINE,     		"TANK_Movement_Slow_Rocky_Terrain_loop_mono",    	1,  true, SndType.SND_FX, 100),
		new SoundProp((int) SndId.SND_TANK_TURRET_ROTATE,   "TANK_Turret_Rotate_01_loop_mono",    	1,  true, SndType.SND_FX, 100),
		new SoundProp((int) SndId.SND_TANK_SHOOT,     		"TANK_Shoot_01",    	1,  false, SndType.SND_FX, 100),

	};

	new void Start()
	{
        base.Start();
	}

	public override int GetByName(string soundName)
	{
		for (int i=0; i<sounds.Length; i++)
		{
			if (String.Compare(sounds[i].name, soundName, true) == 0)
				return sounds[i].id;
		}
		
		return -1;
	}
	
    protected override SoundProp[] GetSoundProps()
    {
        return sounds;
    }

	public override int GetFirstIdNum()
	{
		return 0;
	}

}



