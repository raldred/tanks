// Mono Framework
using System;
using System.Collections;
using System.Xml;
using System.IO;

// Unity Engine
using UnityEngine;

public abstract class SoundList : MonoBehaviour
{
	/// <summary>
	/// All the audioClips, filled in the Unity editor by a programmer or level designer
	/// </summary>
    public AudioClip[] soundClips;
	
	/// <summary>
	/// Loaded from resources
	/// </summary>
	public bool loadedFromResources = true;
	
	/// <summary>
	/// Sound props
	/// </summary>
    protected SoundProp[] _sounds;
	
	/// <summary>
	/// Unity Start method
	/// </summary>
	protected void Start()
	{
        _sounds = GetSoundProps();

        if (_sounds == null)
        {
            Debug.Log("[ERROR] You should define a SoundProp vector at SoundList... class.");
            return;
        }
		
		// New
		if (loadedFromResources)
		{
			soundClips = new AudioClip[_sounds.Length];
			
			for (int i=0; i<_sounds.Length; i++)
			{
				if (_sounds[i].type == SndType.SND_FX)
					soundClips[i] = Resources.Load(string.Format("Sounds/{0}", _sounds[i].name)) as AudioClip;
				else
					soundClips[i] = Resources.Load(string.Format("Music/{0}", _sounds[i].name)) as AudioClip;
			}
		}

		// Relate the array entries with the specified audioClip
		for (int i=0; i<soundClips.Length; i++)
		{
			// Some sounds could be null (if they are not used in the level
			// but the have to keep the sound index for other level)
			if (soundClips[i] != null)
			{
				SoundProp sp = GetSoundPropByName(soundClips[i].name);
			
				if (sp != null)
				{
					sp.audioClip = soundClips[i];
				}
				else
				{
					Debug.LogWarning(String.Format("Cannot find the sound {0} on the array list of sounds.", soundClips[i].name));
				}
			}
		}
	}
	
	/// <summary>
	/// Return the soundprop object specifying its name
	/// </summary>
	public SoundProp GetSoundPropByName(string name)
	{
        for (int i = 0; i < _sounds.Length; i++)
		{
            if (String.Compare(name, _sounds[i].name, true) == 0)
			{
                return _sounds[i];
			}
		}

		return null;
	}

    /// <summary>
    /// Returns the soundprop specifying its id
    /// </summary>
    public SoundProp GetSoundProp(int sndId)
    {
        if (_sounds == null)
        {
            //Debug.Log("ERROR. _sounds is null");
            return null;
        }
		
		sndId = sndId % 1000;
		
        if (sndId < _sounds.Length)
        {
            return _sounds[sndId];
        }
        else
        {
            return null;
        }
    }

    protected abstract SoundProp[] GetSoundProps();
	public abstract int GetByName(string soundName);
	public abstract int GetFirstIdNum();
}



