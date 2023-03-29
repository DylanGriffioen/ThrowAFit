using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public GameObject[] punches = new GameObject[3], throws = new GameObject[2], oofs = new GameObject[3];
    float sfxVolume;
    void Start()
    {
        sfxVolume = MenuSettings.sfxVolume;
    }
    public void Punch()
    {
        var instance = Instantiate(punches[Random.Range(0, punches.Length)],transform);
        instance.GetComponent<AudioSource>().volume *= sfxVolume;
        StartCoroutine(DestroyInstance(instance));
    }
    
    public void TakeDamage()
    {
        var instance = Instantiate(oofs[Random.Range(0, oofs.Length)], transform);
        instance.GetComponent<AudioSource>().volume *= sfxVolume;
        StartCoroutine(DestroyInstance(instance));
    }

    public void Throw()
    {
        var instance = Instantiate(throws[Random.Range(0, throws.Length)], transform);
        instance.GetComponent<AudioSource>().volume *= sfxVolume;
        StartCoroutine(DestroyInstance(instance));
    }

    IEnumerator DestroyInstance(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
