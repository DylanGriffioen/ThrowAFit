using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joining : MonoBehaviour
{
    public GameObject cam;
    CameraMovement cameraMovement;
    void Awake()
    {
        cameraMovement = cam.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*void OnPlayerJoined()
    {
        cameraMovement.FindPlayers();
    }*/
}
