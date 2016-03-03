// Mono Framework
using System;

// Unity Engine
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public abstract class SMAudioSource : MonoBehaviour
{
	private int sndIdInt;
	private SoundList[] soundList;
    bool refreshSoundProps = false;

	void Start()
	{
	}
	
	void OnEnable()
	{
        refreshSoundProps = true;
	}

    void Update()
    {
        if (refreshSoundProps)
        {
            soundList = SoundManager.Instance.GetSoundList();
            RefreshSoundPropProperties();

            refreshSoundProps = false;
        }
    }
	
	/// <summary>
	/// Should be called if sndId is changed dynamically
	/// </summary>
	public void RefreshSoundPropProperties()
	{
		if (soundList == null)
			soundList = SoundManager.Instance.GetSoundList();

        if (soundList == null)
        {
            Debug.LogError("The soundList returned is null.");
            return;
        }

		sndIdInt = getSndIdInt();
		
		SoundProp sp = GetSoundProp(sndIdInt);

        if (sp == null)
        {
            Debug.LogError(string.Format("Cannot find sndId: {0}", sndIdInt));
        }
        else
        {
            gameObject.GetComponent<AudioSource>().clip = sp.audioClip;
            gameObject.GetComponent<AudioSource>().loop = sp.loop;
            gameObject.GetComponent<AudioSource>().pitch = sp.pitch;
            gameObject.GetComponent<AudioSource>().panStereo = sp.pan;
            gameObject.GetComponent<AudioSource>().minDistance = sp.minDistance;
            gameObject.GetComponent<AudioSource>().maxDistance = sp.maxDistance;
			gameObject.GetComponent<AudioSource>().volume = SoundManager.Instance.FxVolume * SoundManager.Instance.MasterVolume * (sp.volume / 100.0f);
        }
	}
	
	
	private SoundProp GetSoundProp(int sndId)
	{
		if (soundList == null)
		{
			soundList = SoundManager.Instance.GetSoundList();
			
			if (soundList == null)
				return null;
		}		
		
		int soundListId = (int)((float)sndId / 1000.0f);
		
		if (soundListId < soundList.Length)
			return soundList[soundListId].GetSoundProp(sndId);
		
		return null;
	}
	
	protected abstract int getSndIdInt();
}