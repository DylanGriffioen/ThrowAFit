using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class DontDestroyPlaySound : MonoBehaviour
{
    private static DontDestroyPlaySound instance = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static DontDestroyPlaySound Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
