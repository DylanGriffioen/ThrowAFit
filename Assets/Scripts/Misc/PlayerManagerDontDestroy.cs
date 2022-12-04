using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerDontDestroy : MonoBehaviour
{
    public static PlayerManagerDontDestroy _instance;

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
