using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw2 : MonoBehaviour
{
    
    public float forwardForce = 20f, angle = 15f, knockback = 2f;
    public bool destroyOnImpact = false;
    [System.NonSerialized] public Transform thrower = null;

    Rigidbody rb;
    Collider coll;
    bool isThrown;
    float upForce;

    void Start()
    {
        coll = GetComponent<Collider>();
        upForce = forwardForce*Mathf.Tan(angle * Mathf.Deg2Rad);
    }
    public void ThrowItem()
    {
        isThrown = true;
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), coll, true);
        rb.AddForce(Mathf.Sqrt(rb.mass)*(transform.forward * forwardForce + Vector3.up * upForce), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) { return; }
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForce(rb.velocity * rb.mass * knockback, ForceMode.Impulse);
        }
        isThrown = false;
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), coll, false);
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
