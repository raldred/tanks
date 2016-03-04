// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public delegate void SoundManagerCallback();

/// <summary>
/// A SoundManager.
/// 
/// Features:
/// . Background, Music and sounds channels.
/// . A priority system for the sound triggering.
/// . Fade In and Fade Out of music and sounds (event oriented)
/// . Fade Out of all sounds (event oriented).
/// 
/// Requires:
/// 
/// A GameObject with this MonoBehaviour, SoundList object and a Dont Destroy on Unload if necessary.
/// </summary>
public class SoundManager : MonoBehaviorSingleton<SoundManager>
{
	/// <summary>
	/// Number of AudioSources to create
	/// </summary>
    public const int AS_COUNT = 16;


    AudioSource[] fx;
    AudioSource music;
    AudioSource musicIntro;
    SoundList[] soundList;
    

    SoundManagerCallback fadeOutAllSoundsCallback;
    bool fadingAllSounds;

    ArrayList asFades = new ArrayList();

    SoundProp currentMusicSndProp = null;
	
	bool playingIntro = false;

	/// <summary>
	/// Unity Awake Method
	/// </summary>
	new void Awake()
	{
		base.Awake();

		Init();
	}
	
    void Init()
    {		
        soundList = GetSoundList();
		
		// Add an specific audiosource for the music
        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        
		musicIntro = gameObject.AddComponent<AudioSource>();
        musicIntro.playOnAwake = false;
		

        // Add many audiosources for multiple sounds
        fx = new AudioSource[AS_COUNT];
        for (int i = 0; i < AS_COUNT; i++)
        {
			fx[i] = gameObject.AddComponent<AudioSource>();
            fx[i].playOnAwake = false;
        }
    }

    /// <summary>
    /// Unity Start Method
    /// </summary>
    void Start()
    {
        MasterVolume = SoundConfigParams.masterVolume;
        MusicVolume = SoundConfigParams.musicVolume;
        FxVolume = SoundConfigParams.fxVolume;

        musicIntro.volume = music.volume = SoundConfigParams.musicVolume * MasterVolume;
        fadingAllSounds = false;

    }

    /// <summary>
    /// Unity OnDisable Method
    /// </summary>
    void OnDisable()
    {
        // Stop all sounds now
        StopAll();
    }

    /// <summary>
    /// Unity Update Method
    /// </summary>
    void Update()
    {
		if (playingIntro)
		{
			if (!musicIntro.isPlaying)
			{
				playingIntro = false;
				music.Play();
			}
		}
		
        if (soundList == null)
		{
	        soundList = GetSoundList();
		}

        // Update the position of the music and the background main sound
        if (Camera.main != null && music != null)
        {
			music.transform.position = Camera.main.transform.position;
        }
		
        // Change the volume of the faded sounds
        foreach (FadeAudioSource fas in asFades)
        {
            fas.accumTime = Time.realtimeSinceStartup - fas.initialTime;

            // Increase the volume
            float totalDelta = fas.targetVolume - fas.initialVolume;
            float deltaVolume = totalDelta * (fas.accumTime / fas.fadeInSecs);

            fas.audioSrc.volume = fas.initialVolume + deltaVolume;

            //Debug.Log(String.Format("totalDelta: {0} deltaVol: {1} vol: {2}", totalDelta, deltaVolume, fas.audioSrc.volume));

            if (fas.accumTime >= fas.fadeInSecs)
            {
                fas.audioSrc.volume = fas.targetVolume;
				
                // Remove the fading
                asFades.Remove(fas);
				
				// Call the event
                if (fas.fnCb != null)
				{
                    fas.fnCb();
				}
				
                // Return on every remove
                return;
            }
        }

        if (fadingAllSounds)
        {
            if (asFades.Count == 0)
            {
                if (fadeOutAllSoundsCallback != null)
                {
                    fadeOutAllSoundsCallback();
                    fadeOutAllSoundsCallback = null;
                }
            }
        }
    }

    /// <summary>
    /// Get/Set Master Volume
    /// </summary>
    public float MasterVolume
    {
        set
        {
            SoundConfigParams.masterVolume = value;
        }
        get
        {
            return SoundConfigParams.masterVolume;
        }
    }

    /// <summary>
    /// Get/Set Music Volume
    /// </summary>
    public float MusicVolume
    {
        set
        {
            SoundConfigParams.musicVolume = value;
            musicIntro.volume = music.volume = value * MasterVolume * (currentMusicSndProp != null ? (currentMusicSndProp.volume / 100.0f) : 1);
        }
        get
        {
            return SoundConfigParams.musicVolume;
        }
    }

    /// <summary>
    /// Get/Set Music Volume
    /// </summary>
    public float FxVolume
    {
        set
        {
            SoundConfigParams.fxVolume = value;

            for (int i = 0; i < AS_COUNT; i++)
            {
                fx[i].volume = value * MasterVolume;
            }
        }
        get
        {
            return SoundConfigParams.fxVolume;
        }
    }

    /// <summary>
    /// Pause music
    /// </summary>
    public void PauseMusic(bool pauseThem)
    {
        if (pauseThem)
        {
            // Pause the background, music and all sounds
            music.Pause();
            musicIntro.Pause();
        }
        else
        {
            if (musicIntro.time > 0)
                musicIntro.Play();
            else if (music.time > 0)
                music.Play();
        }
    }

    /// <summary>
    /// Pause all sounds
    /// </summary>
    public void PauseAll(bool pauseThem)
    {
        if (pauseThem)
        {
            // Pause the background, music and all sounds
            music.Pause();
			musicIntro.Pause();
			
            for (int i = 0; i < AS_COUNT; i++)
                fx[i].Pause();

            // Finish all the fades
            foreach (FadeAudioSource fas in asFades)
                fas.audioSrc.volume = fas.targetVolume;

            asFades.Clear();
        }
        else
        {
            // Resume the background, music and all sounds if there were playing in the pause

            if (musicIntro.time > 0)
				musicIntro.Play();
			else if (music.time > 0)
                music.Play();
			
            for (int i = 0; i < AS_COUNT; i++)
            {
                if (fx[i].time > 0)
                    fx[i].Play();
            }
           

        }
    }

    /// <summary>
    /// Get the specified soundprop or null if it could not be found
    /// </summary>
	public SoundProp GetSoundProp(int sndId)
	{
		if (soundList == null)
		{
			soundList = GetSoundList();
			
			if (soundList == null)
				return null;
		}		
		
		int soundListId = (int)((float)sndId / 1000.0f);
		
		//Debug.Log("SoundListId: " + soundListId + " Length: " + _soundList.Length);

		if (soundListId < soundList.Length)
			return soundList[soundListId].GetSoundProp(sndId);
		
		return null;
	}

    /// <summary>
    /// Play the music
    /// </summary>
    public void PlayMusic(int sndId)
    {
        PlayMusic(sndId, false);
    }

    /// <summary>
    /// Play the music with intro
    /// </summary>
    public void PlayMusic(int introId, int loopId)
    {
		if (music == null) return;

		PlayMusic(loopId);
		music.Pause();
		
        PlayMusic(introId, loopId, false);
    }

    /// <summary>
    /// Player Music with intro and loop
    /// </summary>
    public void PlayMusic(int introId, int loopId, bool fadeOutCurrent)
    {

		if (!SoundConfigParams.useGameMusic) return;

        SoundProp sp = GetSoundProp(introId);
		playingIntro = true;
		//musicLoopId = loopId;
		
        if (sp != null)
        {
            currentMusicSndProp = sp;

            if (fadeOutCurrent && music.isPlaying)
            {
                FadeOutMusic(1, playMusicAfterFadeOut);
            }
            else
            {
                playMusicAfterFadeOut();
            }
        }
		else
		{
			Debug.LogWarning(string.Format ("Cannot find the music id: {0}", introId));
		}
    }
	
    public void PlayMusic(int sndId, bool fadeOutCurrent)
    {
		if (!SoundConfigParams.useGameMusic) return;

        SoundProp sp = GetSoundProp(sndId);

        if (sp != null)
        {
            currentMusicSndProp = sp;

            if (fadeOutCurrent && music.isPlaying)
            {
                FadeOutMusic(1, playMusicAfterFadeOut);
            }
            else
            {
                playMusicAfterFadeOut();
            }
        }
		else
		{
			Debug.LogWarning(string.Format ("Cannot find the music id: {0}", sndId));
		}
    }

    /// <summary>
    /// Returns true if the music if being played
    /// </summary>
    public bool IsPlayingMusic()
    {
        return music.isPlaying;
    }

    private void playMusicAfterFadeOut()
    {
		if (playingIntro)
		{
	        // Set the position of the current camera in order to play the sound balanced
			if (Camera.main != null)
				musicIntro.transform.position = Camera.main.transform.position;
	
	        musicIntro.clip = currentMusicSndProp.audioClip;
	        musicIntro.loop = currentMusicSndProp.loop;
	
	        musicIntro.volume = MusicVolume * MasterVolume * (currentMusicSndProp.volume / 100.0f);
	        musicIntro.Play();		
		}
		else
		{
	        // Set the position of the current camera in order to play the sound balanced
			if (Camera.main != null)
				music.transform.position = Camera.main.transform.position;
	
	        music.clip = currentMusicSndProp.audioClip;
	        music.loop = currentMusicSndProp.loop;
	
	        music.volume = MusicVolume * MasterVolume * (currentMusicSndProp.volume / 100.0f);
	        music.Play();
		}
    }

    /// <summary>
    /// Stop the music
    /// </summary>
    public void StopMusic()
    {
        playingIntro = false;
        music.Stop();
		musicIntro.Stop();
    }

    public void FadeOutMusic(float inSecs, SoundManagerCallback cbfn)
    {
	
        FadeAudioSource fas = new FadeAudioSource();

        fas.initialTime = Time.realtimeSinceStartup;
        fas.accumTime = 0;
		
		if (playingIntro)
		{
	        fas.initialVolume = musicIntro.volume;
    	    fas.audioSrc = musicIntro;
		}
		else
		{
	        fas.initialVolume = music.volume;
    	    fas.audioSrc = music;
		}
		
        fas.fadeInSecs = inSecs;
        fas.targetVolume = 0;
        fas.fnCb += cbfn;

        asFades.Add(fas);
    }

    public void FadeInMusic(int sndId, float inSecs, SoundManagerCallback cbfn)
    {
		if (!SoundConfigParams.useGameMusic) return;
		
        SoundProp sp = GetSoundProp(sndId);

        if (sp != null)
        {
            currentMusicSndProp = sp;

            // Set the position of the current camera in order to play the sound balanced
			if (Camera.main != null)
				music.transform.position = Camera.main.transform.position;

            music.clip = sp.audioClip;
            music.loop = sp.loop;
            music.volume = 0;
            music.Play();

            FadeAudioSource fas = new FadeAudioSource();

            fas.initialTime = Time.realtimeSinceStartup;
            fas.accumTime = 0;
            fas.initialVolume = 0;
            fas.targetVolume = MusicVolume * MasterVolume * (sp.volume / 100.0f);
            fas.audioSrc = music;
            fas.fadeInSecs = inSecs;

            

            if (cbfn != null)
                fas.fnCb += cbfn;
            else
                fas.fnCb = null;

            asFades.Add(fas);
        }
    }

    public int PlaySound<T>(T sndId)
    {
        return PlaySound(Convert.ToInt32(sndId));
    }

    public int PlaySound(int sndId)
    {
		if (Camera.main != null)
        {
			return PlaySound(Camera.main.transform.position, sndId);
        }
        else
		{
            return -1;
		}
    }

    /// <summary>
    /// Play an specific sound
    /// </summary>
    public int PlaySound(Vector3 pos, int sndId)
    {
        if (fx == null) return -1;
        SoundProp sp = GetSoundProp(sndId);
		
        if (sp != null)
        {
            // The specified sound should be marked as FX (the default value)
            if (sp.type == SndType.SND_FX)
            {
                int channeldIdx = getChannelIdx(sp);

                if (channeldIdx != -1)
                {
                    playThisSoundOnSource(channeldIdx, sp, pos);
                    return channeldIdx;
                }
                else
                    Debug.Log("All audiosource are busy. Cannot play sound: " + sp.name);
            }
            else
                Debug.Log(String.Format("Trying to play a sound that is not a FX ({0})", sp.name));
        }
		else
		{
			Debug.LogWarning(string.Format("Cannot find the fx: {0}", sndId));
		}

        return -1;
    }

    void playThisSoundOnSource(int idx, SoundProp sp, Vector3 pos)
    {
        fx[idx].Stop();
        fx[idx].clip = sp.audioClip;
        fx[idx].loop = sp.loop;
        fx[idx].volume = FxVolume * MasterVolume * (sp.volume / 100.0f);

        fx[idx].transform.position = pos;
        fx[idx].Play();
    }

    /// <summary>
    /// Returns a channeld idx to play a sound.
    /// Could be:
    /// 1. An empty channel (not yet used)
    /// 2. An IDLE channel
    /// 3. A busy channel but with less priority
    /// 4. A busy channel with the same priority
    /// 
    /// If there isn't a channel that satisfy these conditions, returns -1
    /// 
    /// </summary>
    /// <returns></returns>
    int getChannelIdx(SoundProp sp)
    {
        for (int i = 1; i < AS_COUNT; i++)
        {
            if (fx[i].clip != null)
            {
                // Found a audiosource that is not currently being played
                if (!fx[i].isPlaying)
                {
                    //if (_fx[i].clip != sp.audioClip)
                    {
                        return i;
                    }
                }
            }
            else
            {
                return i;
            }
        }

        // No audiosource idle. Find a busy audiosource with less priority than the new one
        for (int i = 0; i < AS_COUNT; i++)
        {
            if (fx[i].clip != null)
            {
                SoundProp prop = getSoundPropByName(fx[i].clip.name);
				
                if (sp.priority > prop.priority)
				{
					Debug.Log("Returning a used channel");
                    return i;
				}
            }
        }

        // Try something with the same priority
        for (int i = 0; i < AS_COUNT; i++)
        {
            if (fx[i].clip != null)
            {
                SoundProp prop = getSoundPropByName(fx[i].clip.name);
                if (sp.priority == prop.priority)
				{
					//Debug.Log("Returning a used channel2");
                    return i;
				}
            }
        }

        // Cannot find a suitable channel
        return -1;
    }
	
	SoundProp getSoundPropByName(string name)
	{
		if (soundList == null)
		{
			soundList = GetSoundList();
			
			if (soundList == null)
				return null;
		}
		
		foreach(SoundList list in soundList)
		{
			SoundProp prop = list.GetSoundPropByName(name);
			
			if (prop != null)
				return prop;
		}
		
		return null;
	}

    /// <summary>
    /// Stop an specific sound immediatelly
    /// </summary>
    public void StopSound(int channelIdx)
    {
        if (channelIdx != -1 && channelIdx < AS_COUNT)
        {
            if (fx[channelIdx] != null && fx[channelIdx].clip != null)
            {
                fx[channelIdx].Stop();
            }
        }
    }

    /// <summary>
    /// Fade out an specific sound.
    /// Do not call other function of the sound manager that requires a callback function before the first
    /// one finishes and call the callback.
    /// </summary>
    public void FadeOutSound(int channelIdx, float inSecs, SoundManagerCallback cbfn)
    {
        FadeAudioSource fas = new FadeAudioSource();

        fas.initialTime = Time.realtimeSinceStartup;
        fas.accumTime = 0;
        fas.initialVolume = fx[channelIdx].volume;
        fas.targetVolume = 0;
        fas.audioSrc = fx[channelIdx];
        fas.fadeInSecs = inSecs;
        fas.fnCb += cbfn;

        asFades.Add(fas);
    }

    public void FadeInSound(int sndId, float inSecs, SoundManagerCallback cbfn)
    {
		//if (!ConfigParams.useGameMusic) return;
        SoundProp sp = GetSoundProp(sndId);

        if (sp != null)
        {

            int channeldIdx = getChannelIdx(sp);

            if (channeldIdx != -1)
            {
                // Set the position of the current camera in order to play the sound balanced
				if (Camera.main != null)
					fx[channeldIdx].transform.position = Camera.main.transform.position;

                fx[channeldIdx].clip = sp.audioClip;
                fx[channeldIdx].loop = sp.loop;
                fx[channeldIdx].volume = 0;
                fx[channeldIdx].Play();

                FadeAudioSource fas = new FadeAudioSource();

                fas.initialTime = Time.realtimeSinceStartup;
                fas.accumTime = 0;
                fas.initialVolume = 0;
                fas.targetVolume = FxVolume * MasterVolume * (sp.volume / 100.0f);
                fas.audioSrc = fx[channeldIdx];
                fas.fadeInSecs = inSecs;

                if (cbfn != null)
                    fas.fnCb += cbfn;
                else
                    fas.fnCb = null;

                asFades.Add(fas);
            }

        }
    }

    /// <summary>
    /// Stops all the sounds immediatelly
    /// </summary>
    public void StopAll()
    {
        if (music)
            music.Stop();
		
		if (musicIntro)
			musicIntro.Stop();

        for (int i = 0; i < AS_COUNT; i++)
        {
            if (fx != null)
                if (fx[i] != null)
                    fx[i].Stop();
        }

        asFades.Clear();
    }

    /// <summary>
    /// Fade out all the sounds that are currently being played.
    /// </summary>
    /// <param name="fncb"></param>
    public void FadeOutAll(float inSecs, SoundManagerCallback cbfn)
    {

        // Finish all the fades
        foreach (FadeAudioSource fas in asFades)
            fas.audioSrc.volume = fas.targetVolume;

        asFades.Clear();

        FadeOutMusic(inSecs, null);

        for (int i=0; i<AS_COUNT; i++)
        {
            if (fx[i].isPlaying)
                FadeOutSound(i, inSecs, null);
        }

        fadingAllSounds = true;
        fadeOutAllSoundsCallback += cbfn;
    }

    /// <summary>
    /// Return the channel
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AudioSource GetChannelById(int id)
    {
        return fx[id];
    }

	/// <summary>
	/// Returns the associated sound list
	/// </summary>
    public SoundList[] GetSoundList()
    {
		soundList = null;
		
        SoundList[] sl = gameObject.GetComponents<SoundList>();
		
		if (sl != null)
		{
			soundList = new SoundList[sl.Length];
			
			//Debug.Log("SoundList Length: " + _soundList.Length);
			
			foreach(SoundList s in sl)
			{
				int id = (int)((float)s.GetFirstIdNum() / 1000.0f);
				//Debug.Log("SoundList ID: " + id);
	       		soundList[id] = s;
			}
		}
		else
		{
			Debug.LogError("No sound lists found!");
		}

		return soundList;
    }
		
}

