using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector3 origPos;
    Rigidbody rb;
    void Start()
    {
        origPos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < 2.5f)
        {
            Health health = gameObject.GetComponent<Health>();
            if(health != null)
            {
                health.Kill();
            }
        }
    }

    public void RespawnPlayer()
    {
        transform.position = origPos;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
    public void RespawnPlayer(Vector3 pos)
    {
        transform.position = pos;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
}
