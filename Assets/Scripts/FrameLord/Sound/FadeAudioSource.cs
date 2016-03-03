// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public class FadeAudioSource
{
    public float targetVolume;          // Target volume to go
    public float initialVolume;         // Initial volume
    public float fadeInSecs;            // Complete the fade in the specified seconds
    public float initialTime;
    public float accumTime;             // Current delta time
    public AudioSource audioSrc;        // The affected audiosource
    public SoundManagerCallback fnCb;
}
