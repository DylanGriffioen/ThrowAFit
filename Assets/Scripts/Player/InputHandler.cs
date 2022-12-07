using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;

    [SerializeField] float punchDamage = 15;
    [SerializeField] float punchAnimationTime = 0.5f;
    [SerializeField] float punchForce = 1f;

    [SerializeField] float kickDamage = 30;
    [SerializeField] float kickAnimationTime = 1f;
    [SerializeField] float kickForce = 2f;

    [SerializeField] float hitImpulseAngle = 15f;

    private float _forceMultiplier = 1f;
    private float _damageMultiplier = 1f;
    void Start()
    {
        if (GameManager._instance != null)
        {
            _forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : _forceMultiplier;
            _damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : _damageMultiplier;
        }
    }

    private void Update()
    {
        if (GameManager._instance != null && GameManager.GAME_STATE == GameStates.PREGAME)
        {
            _forceMultiplier = GameManager._instance.ForceMultiplier > 0 ? GameManager._instance.ForceMultiplier : _forceMultiplier;
            _damageMultiplier = GameManager._instance.DamageMultiplier > 0 ? GameManager._instance.DamageMultiplier : _damageMultiplier;
        }
    }


    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        Debug.Log("PAUSE!");
        if (GameManager.GAME_STATE != GameStates.GAME)
            return;

        GameObject pauseScreen = GameManager._instance.GameCanvas.transform.GetChild(0).gameObject;
        if (pauseScreen != null)
            return;

        if (GameManager.GAME_STATE != GameStates.PAUSE)
        {
            Debug.Log("Game paused!");
            Time.timeScale = 0;
            GameManager.GAME_STATE = GameStates.PAUSE;
            pauseScreen.SetActive(true);
        }
        else
        {
            Debug.Log("Game unpaused!");
            Time.timeScale = 1;
            GameManager.GAME_STATE = GameStates.GAME;
            pauseScreen.SetActive(false);
        }
    }

    public void OnPunch(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        string[] punchAnimations = { "Punch", "Punch2" };


        Debug.Log("Punch!");
        playerAnimator.SetTrigger(punchAnimations[Random.Range(0, punchAnimations.Length)]);
        StartCoroutine(FinishHit(punchAnimationTime, punchDamage, punchForce));
    }

    public void OnKick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        Debug.Log("Kick!");
        playerAnimator.SetTrigger("Kick");
        StartCoroutine(FinishHit(kickAnimationTime, kickDamage, kickForce));
    }


    IEnumerator FinishHit(float animationTime, float damage, float force)
    {
        yield return new WaitForSeconds(animationTime);

        List<GameObject> objectsInFront = new List<GameObject>();
        float objectDistance;
        float closestDistance = (objectsInFront[0].transform.position - transform.parent.position).magnitude;
        int closestIndex = 0;
        for (int i = 1; i < objectsInFront.Count; i++)
        {
            objectDistance = (objectsInFront[i].transform.position - transform.parent.position).magnitude;
            if (objectDistance < closestDistance)
            {
                closestDistance = objectDistance;
                closestIndex = i;
            }
        }

        GameObject closestObject = objectsInFront[closestIndex].gameObject;
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


}
