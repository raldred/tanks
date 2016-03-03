// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public class SoundConfigParams
{
	/// <summary>
	/// Use Game Music
	/// </summary>
	public static bool useGameMusic = true;

	/// <summary>
	/// Master Volume
	/// </summary>
	public static float masterVolume = 1.0f;

	/// <summary>
	/// Music Volume
	/// </summary>
	public static float musicVolume = 1.0f;

	/// <summary>
	/// FX sounds Volume
	/// </summary>
	public static float fxVolume = 1.0f;

	/// <summary>
	/// Save Settings
	/// </summary>
    public static void SaveSettings()
    {
		PlayerPrefs.SetInt("UseGameMusic", (useGameMusic ? 1 : 0));
		PlayerPrefs.SetFloat("MasterVolume", masterVolume);
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat("FxVolume", fxVolume);
    }

    /// <summary>
    /// Load Settings
    /// </summary>
    public static void LoadSettings()
    {
		useGameMusic = (PlayerPrefs.GetInt("UseGameMusic", 1) == 1 ? true : false);
		masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
		musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
		fxVolume = PlayerPrefs.GetFloat("FxVolume", 1.0f);
    }
	

}
