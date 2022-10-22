using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw2 : MonoBehaviour
{


    Rigidbody rb;
    Collider collider;
    public float forwardForce = 20f, upForce = 4f, knockback = 2f;
    public bool destroyOnImpact = false;
    [System.NonSerialized] public Transform thrower = null;
    Transform prevThrower = null;


    float throwTimer = 0f;
    bool isThrown;
    private bool ignoreEnds;

    void Start()
    {
        collider = GetComponent<Collider>();
    }
    public void ThrowItem()
    {
        isThrown = true;
        rb = GetComponent<Rigidbody>();
        throwTimer = 0f;
        ignoreEnds = false;
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), collider, true);
        rb.AddForce(Mathf.Sqrt(rb.mass)*(transform.forward * forwardForce + Vector3.up * upForce), ForceMode.Impulse);
    }

    void Update()
    {
        ItemThrown();
    }

    void ItemThrown()
    {
        if (isThrown && !ignoreEnds)
        {
            if (throwTimer < 0.1f)
            {
                throwTimer += Time.deltaTime;
            }
            else
            {
                print("Here!");
                ignoreEnds = true;
                Physics.IgnoreCollision(thrower.GetComponent<Collider>(), collider, false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            if (collision.rigidbody != null)
            {
                collision.rigidbody.AddForce(rb.velocity * rb.mass * knockback, ForceMode.Impulse);
            }
            isThrown = false;
        }
    }
}
