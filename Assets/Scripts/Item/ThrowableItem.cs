using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    public float forwardForce = 20f, angle = 15f, knockback = 2f;
    static float hitImpulseAngle = 45f;
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
            var collRB = collision.rigidbody;
            var collGO = collision.gameObject;
            if (collGO.CompareTag("Player"))
            {
                var movementScript = collGO.GetComponent<Movement>();
                movementScript.ObjectHitPlayer();
            }
            var impulseVelocityXZ = new Vector2(rb.velocity.x, rb.velocity.z) * rb.mass * knockback;
            var impulseVelocityY = impulseVelocityXZ.magnitude * Mathf.Tan(hitImpulseAngle * Mathf.Deg2Rad);
            print(impulseVelocityXZ);
            print(impulseVelocityY);
            var impulseVelocity = new Vector3(impulseVelocityXZ.x, impulseVelocityY, impulseVelocityXZ.y);
            collRB.velocity = new Vector3(collRB.velocity.x, 0f, collRB.velocity.z);
            collRB.AddForce(impulseVelocity, ForceMode.Impulse);
            
        }
        isThrown = false;
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), coll, false);
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
        
    }
}
