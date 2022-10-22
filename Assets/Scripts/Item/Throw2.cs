using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw2 : MonoBehaviour
{
    Rigidbody rb;
    Collider collider;
    public float forwardForce = 10f, upForce = 3f, knockback = 2f;
    public bool destroyOnImpact = false;
    [System.NonSerialized] public Transform thrower = null;
    Transform prevThrower = null;

    bool isThrown;

    void Start()
    {
        collider = GetComponent<Collider>();

    }
    public void ThrowItem()
    {
        isThrown = true;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * forwardForce + Vector3.up * upForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (thrower != prevThrower)
        {
            if (prevThrower != null)
            {
                Physics.IgnoreCollision(prevThrower.GetComponent<Collider>(), collider, false);
            }
            Physics.IgnoreCollision(thrower.GetComponent<Collider>(), collider, true);
        }
        prevThrower = thrower;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            if (collision.rigidbody != null)
            {
                collision.rigidbody.AddForce(rb.velocity * knockback, ForceMode.Impulse);
            }
            isThrown = false;
        }
    }
}
