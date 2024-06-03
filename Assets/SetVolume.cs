using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
    public bool music;
    void Start()
    {
        GetComponent<AudioSource>().volume *= music ? MenuSettings.musicVolume : MenuSettings.sfxVolume;
    }
}
