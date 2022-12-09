using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PunchKick : MonoBehaviour
{
    private bool _canPunchKick = true;

    PlaySFX sfxPlayer;
    Animator animator;

    [SerializeField] float punchDamage = 15;
    [SerializeField] float punchTime = 0.5f;
    [SerializeField] float punchEndLag = 0.2f;
    [SerializeField] float punchForce = 1f;

    [SerializeField] float kickDamage = 30;
    [SerializeField] float kickTime = 0.5f;
    [SerializeField] float kickEndLag = 0.2f;
    [SerializeField] float kickForce = 2f;

    [SerializeField] float hitImpulseAngle = 15f;

    private float _forceMultiplier = 1f;
    private float _damageMultiplier = 1f;

    List<GameObject> objectsInHitbox = new List<GameObject>();

    public List<GameObject> ObjectsInHitbox { get { return objectsInHitbox; } }

    Movement movementScript;

    bool animating, hitting;
    float animationHitTime, animationEndLag, animationTotalTime, timer, animationSplit;
    void Awake()
    {
        movementScript = transform.parent.GetComponent<Movement>();
        animator = transform.parent.GetComponentInChildren<Animator>();
        sfxPlayer = GameObject.Find("SFX").GetComponent<PlaySFX>();
    }
    void Start()
    {
        UpdateMultipliers();
    }

    void Update()
    {
        UpdateMultipliers();
        AnimationTime();
    }
    void AnimationTime()
    {
        if (animating)
        {
            timer += Time.deltaTime;
            if (timer < animationHitTime)
            {
                animator.SetFloat("Animation Timer", (timer/animationHitTime)*animationSplit);
            }
            else if (timer < animationTotalTime)
            {
                animator.SetFloat("Animation Timer", animationSplit + (timer - animationHitTime) / animationEndLag * (1 - animationSplit));
            }
            else
            {
                animator.SetFloat("Animation Timer", 1f);
                animating = false;
            }
        }
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
        if (!_canPunchKick)
            return;
        if (!ctx.performed || hitting || !movementScript.movementEnabled || movementScript.holdingItem)
            return;
        hitting = true;
        movementScript.movementEnabled = false;
        string[] punchAnimations = { "Punch", "Punch2" };
        var randInt = Random.Range(0, punchAnimations.Length);
        animator.SetTrigger(punchAnimations[randInt]);
        animationSplit = randInt == 0 ? 0.55f : 0.7f;
        StartCoroutine(Hit(punchTime, punchEndLag, punchDamage, punchForce));
        animator.SetBool("Idling Enabled", false);
    }
    public void OnKick(InputAction.CallbackContext ctx)
    {
        if (!_canPunchKick)
            return;
        if (!ctx.performed || hitting || !movementScript.movementEnabled || movementScript.holdingItem)
            return;
        hitting = true;
        movementScript.movementEnabled = false;
        animator.SetTrigger("Kick");
        animationSplit = 0.45f;
        StartCoroutine(Hit(kickTime, kickEndLag, kickDamage, kickForce));
        animator.SetBool("Idling Enabled", false);
    }
    IEnumerator Hit(float hitTime, float endLag, float damage, float force)
    {
        //Animate
        animating = true;
        animationHitTime = hitTime;
        animationEndLag = endLag;
        animationTotalTime = hitTime + endLag;
        timer = 0f;

        yield return new WaitForSeconds(hitTime);
        
        //Remove Destroyed objects
        for (int i = objectsInHitbox.Count-1; i >= 0; i--)
        {
            if (objectsInHitbox[i] == null)
            {
                objectsInHitbox.RemoveAt(i);
                print("RemovedCuzNull");
            }
        }

        //Apply things to each object in area
        foreach (GameObject obj in objectsInHitbox)
        {
            var hitRB = obj.GetComponent<Rigidbody>();
            if (hitRB == null) { continue; }
            Vector2 impulseVelocityXZ = new Vector2(transform.forward.x,transform.forward.z) * force * _forceMultiplier;
            float impulseVelocityY = impulseVelocityXZ.magnitude * Mathf.Tan(hitImpulseAngle * Mathf.Deg2Rad);

            if (obj.CompareTag("Player"))
            {
                var movementScript = obj.GetComponent<Movement>();
                movementScript.ObjectHitPlayer();

                Health playerHealth = obj.GetComponent<Health>();
                if (playerHealth != null)
                {
                    impulseVelocityXZ *= playerHealth.GetHitForceMultiplier();
                    playerHealth.Damage(damage * _damageMultiplier);
                }
            }
            //print(impulseVelocityXZ);
            //print(impulseVelocityY);
            var impulseVelocity = new Vector3(impulseVelocityXZ.x, impulseVelocityY, impulseVelocityXZ.y);
            hitRB.velocity = new Vector3(hitRB.velocity.x, 0f, hitRB.velocity.z);
            hitRB.AddForce(impulseVelocity, ForceMode.Impulse);
            sfxPlayer.Punch();
        }
        yield return new WaitForSeconds(endLag);

        //End hit
        movementScript.movementEnabled = true;
        hitting = false;
        animator.SetBool("Idling Enabled", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        print("Added");
        objectsInHitbox.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        print("Removed");
        objectsInHitbox.Remove(other.gameObject);
    }

}
