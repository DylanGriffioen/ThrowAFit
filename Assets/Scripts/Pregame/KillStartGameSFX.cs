using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillStartGameSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("KillSoundObject", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void KillSoundObject()
    {
        //Destroy(DontDestroyPlaySound.Instance.gameObject);
    }
}
