using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    public float forwardForce = 20f, angle = 15f, knockback = 2f;
    static float hitImpulseAngle = 45f;
    [SerializeField] float onHitDamage = 100f;
    public bool destroyOnImpact = false;
    [System.NonSerialized] public Transform thrower = null;

    Rigidbody rb;
    Collider coll;
    bool isThrown;
    float upForce;

    private float _forceMultiplier = 1f;
    private float _damageMultiplier = 1f;


    void Start()
    {
        if(GameManager._instance != null)
        {
            _forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : _forceMultiplier;
            _damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : _damageMultiplier;
        }
        coll = GetComponent<Collider>();
        upForce = forwardForce*Mathf.Tan(angle * Mathf.Deg2Rad);
    }

    private void Update()
    {
        if (GameManager._instance != null && GameManager.GAME_STATE == GameStatus.PREGAME)
        {
            _forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : _forceMultiplier;
            _damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : _damageMultiplier;
        }
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
        if (collision.rigidbody != null && !collision.gameObject.CompareTag("Ragdoll"))
        {
            var collRB = collision.rigidbody;
            var collGO = collision.gameObject;
            var impulseVelocityXZ = new Vector2(rb.velocity.x, rb.velocity.z) * knockback * _forceMultiplier;
            if (rb != null)
                impulseVelocityXZ *= rb.mass;

            var impulseVelocityY = impulseVelocityXZ.magnitude * Mathf.Tan(hitImpulseAngle * Mathf.Deg2Rad);
            if (collGO.CompareTag("Player"))
            {
                var movementScript = collGO.GetComponent<Movement>();
                movementScript.ObjectHitPlayer();

                Health playerHealth = collGO.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(onHitDamage * _damageMultiplier);
                    impulseVelocityXZ *= playerHealth.GetHitForceMultiplier();
                }
            }
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
