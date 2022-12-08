using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    Movement movementScript;
    Animator animator;
    public bool onGround;
    bool triggering, onGroundLastFrame;
    void Awake()
    {
        movementScript = transform.GetComponentInParent<Movement>();
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        onGround = triggering;
        if (onGround != onGroundLastFrame)
        {
            movementScript.onGround = onGround;
            animator.SetBool("On Ground", onGround);
        }
        onGroundLastFrame = onGround;
    }

    private void FixedUpdate()
    {
        triggering = false;
    }

    private void OnTriggerStay(Collider other)
    {
        triggering = true;
    }
}
