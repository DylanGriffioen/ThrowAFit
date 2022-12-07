using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerSingleton : MonoBehaviour
{
    public static PlayerManagerSingleton _instance;

    void Awake()
    {
        MakeSingleton();
    }
    private void MakeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != null)
        {
            Destroy(gameObject);
        }
    }

 }
