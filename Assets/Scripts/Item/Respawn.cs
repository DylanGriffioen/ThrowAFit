using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector3 origPos;
    void Start()
    {
        origPos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < 2.5f)
        {
            transform.position = origPos;
        }
    }
}
