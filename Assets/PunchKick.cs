using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PunchKick : MonoBehaviour
{
    Animator animator;

    [SerializeField] float punchDamage = 15;
    [SerializeField] float punchAnimationTime = 0.5f;
    [SerializeField] float punchForce = 1f;

    [SerializeField] float kickDamage = 30;
    [SerializeField] float kickAnimationTime = 1f;
    [SerializeField] float kickForce = 2f;

    [SerializeField] float hitImpulseAngle = 15f;

    private float _forceMultiplier = 1f;
    private float _damageMultiplier = 1f;

    List<GameObject> objectsInHitbox = new List<GameObject>();

    Movement movementScript;
    void Awake()
    {
        movementScript = transform.parent.GetComponent<Movement>();
        animator = transform.parent.GetComponentInChildren<Animator>();
    }
    void Start()
    {
        UpdateMultipliers();
    }

    void Update()
    {
        UpdateMultipliers();
    }

    void UpdateMultipliers()
    {
        if (GameManager._instance != null && GameManager.GAME_STATE == GameStatus.PREGAME)
        {
            if (GameManager._instance.ForceMultiplier > 0)
            {
                _forceMultiplier = GameManager._instance.ForceMultiplier;
            }
            if (GameManager._instance.DamageMultiplier > 0)
            {
                _damageMultiplier = GameManager._instance.DamageMultiplier;
            }
        }
    }

    public void OnPunch(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        movementScript.movementEnabled = false;
        string[] punchAnimations = { "Punch", "Punch2" };

        Debug.Log("Punch!");
        animator.SetTrigger(punchAnimations[Random.Range(0, punchAnimations.Length)]);
        StartCoroutine(FinishHit(punchAnimationTime, punchDamage, punchForce));
    }

    public void OnKick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        movementScript.movementEnabled = false;
        Debug.Log("Kick!");
        animator.SetTrigger("Kick");
        StartCoroutine(FinishHit(kickAnimationTime, kickDamage, kickForce));
    }


    IEnumerator FinishHit(float animationTime, float damage, float force)
    {
        yield return new WaitForSeconds(animationTime);

        movementScript.movementEnabled = true;
        objectsInHitbox.Clear();
        float objectDistance;
        if(objectsInHitbox[0] == null)
        {
            objectsInHitbox.Clear();
            yield break;
        }
        float closestDistance = (objectsInHitbox[0].transform.position - transform.parent.position).magnitude;
        int closestIndex = 0;
        for (int i = 1; i < objectsInHitbox.Count; i++)
        {
            objectDistance = (objectsInHitbox[i].transform.position - transform.parent.position).magnitude;
            if (objectDistance < closestDistance)
            {
                closestDistance = objectDistance;
                closestIndex = i;
            }
        }

        GameObject closestObject = objectsInHitbox[closestIndex].gameObject;
        Debug.Log("closest Object: " + closestObject);

        if (closestObject != null)
        {
            Rigidbody closestRB = closestObject.GetComponent<Rigidbody>();

            if (closestRB != null)
            {

                Vector3 impulseVelocityXZ = gameObject.transform.forward * force * _forceMultiplier;
                float impulseVelocityY = impulseVelocityXZ.magnitude * Mathf.Tan(hitImpulseAngle * Mathf.Deg2Rad);

                if (closestObject.CompareTag("Player"))
                {
                    var movementScript = closestObject.GetComponent<Movement>();
                    movementScript.ObjectHitPlayer();

                    Health playerHealth = closestObject.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.Damage(damage * _damageMultiplier);
                        impulseVelocityXZ *= playerHealth.GetHitForceMultiplier();
                    }
                }
                print(impulseVelocityXZ);
                print(impulseVelocityY);
                var impulseVelocity = new Vector3(impulseVelocityXZ.x, impulseVelocityY, impulseVelocityXZ.y);
                closestRB.velocity = new Vector3(closestRB.velocity.x, 0f, closestRB.velocity.z);
                closestRB.AddForce(impulseVelocity, ForceMode.Impulse);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        objectsInHitbox.Add(other.gameObject);
    }


    private void OnTriggerExit(Collider other)
    {
        objectsInHitbox.Remove(other.gameObject);
    }
}
