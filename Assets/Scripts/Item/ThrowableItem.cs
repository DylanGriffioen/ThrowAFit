using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    
    public float forwardForce = 20f, angle = 15f, knockback = 2f;
    [SerializeField] float forceMultiplier = 1f;

    [SerializeField] float onHitDamage = 100f;
    [SerializeField] float damageMultiplier = 1f;

    public bool destroyOnImpact = false;
    [System.NonSerialized] public Transform thrower = null;

    Rigidbody rb;
    Collider coll;
    bool isThrown;
    float upForce;

    void Start()
    {
        forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : forceMultiplier;
        damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : damageMultiplier;
        coll = GetComponent<Collider>();
        upForce = forwardForce*Mathf.Tan(angle * Mathf.Deg2Rad);
    }
    private void Update()
    {
        if (GameManager.GAME_STATE == GameStates.PREGAME)
        {
            forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : forceMultiplier;
            damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : damageMultiplier;
        }
    }
    public void ThrowItem()
    {
        isThrown = true;
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), coll, true);
        rb.AddForce(Mathf.Sqrt(rb.mass)*(transform.forward * forwardForce * forceMultiplier + Vector3.up * upForce), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) { return; }
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForce(rb.velocity * rb.mass * knockback, ForceMode.Impulse);

            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.Damage(onHitDamage*damageMultiplier);
            }
        }
        isThrown = false;
        Physics.IgnoreCollision(thrower.GetComponent<Collider>(), coll, false);
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
