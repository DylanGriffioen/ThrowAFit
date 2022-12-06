using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillStartGameSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillSoundObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator KillSoundObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(DontDestroyPlaySound.Instance.gameObject);
    }
}
