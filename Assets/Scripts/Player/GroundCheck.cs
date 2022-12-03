using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    Movement movementScript;
    Animator animator;
    bool onGround;
    void Awake()
    {
        movementScript = transform.GetComponentInParent<Movement>();
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (onGround) { return; }
        movementScript.onGround = true;
        animator.SetBool("On Ground", true);
        onGround = true;
    }
    private void OnTriggerExit(Collider other)
    {
        onGround = false;
        movementScript.onGround = false;
        animator.SetBool("On Ground", false);
    }
}
