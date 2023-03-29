using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSettings : MonoBehaviour
{
    public static float musicVolume = 0.5f, sfxVolume = 0.5f;
    public AudioSource musicPlayer, select, hover, back;
    
    AudioSource[] sfxPlayers;
    float[] relativeVolumes;
    void Start()
    {
        sfxPlayers = new AudioSource[] { select, hover, back };
        relativeVolumes = new float[sfxPlayers.Length];
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            relativeVolumes[i] = sfxPlayers[i].volume;
            sfxPlayers[i].volume = relativeVolumes[i] * sfxVolume;
        }
    }
    public void SetMusicVolume(float value)
    {
        float volume = 0.5f*Mathf.Clamp(value, 0, 1);
        musicVolume = volume;
        musicPlayer.volume = volume;
    }
    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp(value, 0, 1);
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = relativeVolumes[i] * sfxVolume;
        }
    }
}
