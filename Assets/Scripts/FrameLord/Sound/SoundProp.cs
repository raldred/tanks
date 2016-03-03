using UnityEngine;
using System.Collections;

public class SoundProp
{
    const string DEF_NAME = "";
    const int DEF_PRIORITY = 128;
    const int DEF_VOLUME = 100;
    const SndType DEF_SND_TYPE = SndType.SND_FX;
    const bool DEF_LOOP_VALUE = false;

    const float DEF_PITCH = 1.0f;
    const float DEF_PAN = 0.0f;
    const float DEF_MIN_DISTANCE = 1.0f;
    const float DEF_MAX_DISTANCE = 200.0f;


    /// <summary>
    /// Id of the sound
    /// </summary>
    public int id;

    /// <summary>
    /// Name of the clip
    /// </summary>
    public string name;

    /// <summary>
    /// Sound or music
    /// </summary>
    public SndType type;

    /// <summary>
    /// Priority.
    /// </summary>
    public int priority;

    /// <summary>
    /// Related audio clip
    /// </summary>
    public AudioClip audioClip;

    /// <summary>
    /// Looped?
    /// </summary>
    public bool loop;

    /// <summary>
    /// The volume of the clip.
    /// Possible values: 0 to 100 -> Converted to 0.0 to 1.0f when it's used
    /// </summary>
    public int volume;

    /// <summary>
    /// The pitch.
    /// </summary>
    public float pitch;

    /// <summary>
    /// The pan.
    /// </summary>
    public float pan;

    /// <summary>
    /// The minimum distance.
    /// </summary>
    public float minDistance;

    /// <summary>
    /// The max distance.
    /// </summary>
    public float maxDistance;

    public SoundProp()
    {
        this.priority = DEF_PRIORITY;
        this.type = DEF_SND_TYPE;
        this.loop = DEF_LOOP_VALUE;
        this.volume = DEF_VOLUME;
        this.pitch = DEF_PITCH;
        this.pan = DEF_PAN;
        this.minDistance = DEF_MIN_DISTANCE;
        this.maxDistance = DEF_MAX_DISTANCE;
    }

    public SoundProp(int sndId, string name, int priority)
    {
        this.id = sndId;
        this.name = name;
        this.priority = priority;
        this.type = DEF_SND_TYPE;
        this.loop = DEF_LOOP_VALUE;
        this.volume = DEF_VOLUME;
        this.pitch = DEF_PITCH;
        this.pan = DEF_PAN;
        this.minDistance = DEF_MIN_DISTANCE;
        this.maxDistance = DEF_MAX_DISTANCE;
    }

    public SoundProp(int sndId, string name, int priority, int volume)
    {
        this.id = sndId;
        this.name = name;
        this.priority = priority;
        this.loop = DEF_LOOP_VALUE;
        this.type = DEF_SND_TYPE;
        this.volume = volume;
        this.pitch = DEF_PITCH;
        this.pan = DEF_PAN;
        this.minDistance = DEF_MIN_DISTANCE;
        this.maxDistance = DEF_MAX_DISTANCE;
    }

    public SoundProp(int sndId, string name, int priority, bool loop, SndType type)
    {
        this.id = sndId;
        this.name = name;
        this.priority = priority;
        this.loop = loop;
        this.type = type;
        this.volume = DEF_VOLUME;
        this.pitch = DEF_PITCH;
        this.pan = DEF_PAN;
        this.minDistance = DEF_MIN_DISTANCE;
        this.maxDistance = DEF_MAX_DISTANCE;
    }

    public SoundProp(int sndId, string name, int priority, bool loop, SndType type, int volume)
    {
        this.id = sndId;
        this.name = name;
        this.priority = priority;
        this.loop = loop;
        this.type = type;
        this.volume = volume;
        this.pitch = DEF_PITCH;
        this.pan = DEF_PAN;
        this.minDistance = DEF_MIN_DISTANCE;
        this.maxDistance = DEF_MAX_DISTANCE;
    }

}
