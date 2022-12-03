using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float baseMoveSpeed, jumpHeight, throwMoveImpulse, throwEndLag, smoothTurnTime;

    Rigidbody rb;
    Transform model;
    Animator animator;

    Vector3 origScale, origLocalPos;
    Vector2 inputDir, moveDir;
    bool crouching, movePressed, moving, movingLastFrame, jumping, falling, jumpingLeftGround, throwing;
    bool movementEnabled = true;
    [System.NonSerialized] public bool holdingItem, onGround;
    [System.NonSerialized] public float heldItemMass = 1f;
    float targetAngle, smoothAngle, smoothTurnVelocity, moveSpeed, moveSpeedMult, jumpSpeed, jumpHeightMult, jumpPercent, speedMultLastFrame, gravity;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity.magnitude;

        model = transform.GetChild(0);
        origScale = model.localScale;
        origLocalPos = model.localPosition;
        animator = model.GetComponent<Animator>();
    }

    void Update()
    {
        Initial();
        Directional();
        Jumping();
    }
    void Initial()
    {
        //Move
        moving = movePressed && movementEnabled;
        if (moving != movingLastFrame)
        {
            animator.SetBool("Moving", moving);
        }
        movingLastFrame = moving;

        //Jump
        if (!jumping)
        {
            if (!onGround && !falling)
            {
                falling = true;
                animator.SetTrigger("Fall");
            }
            if (falling && onGround)
            {
                falling = false;
            }
        }
        jumpHeightMult = 1f;
        if (holdingItem && heldItemMass > 1f) { jumpHeightMult /= heldItemMass; } //If holding item, divide jump speed multiplier by mass of held item
        
    }
    void Directional()
    {
        if (!moving) { return; }

        //Directional movement
        moveSpeedMult = crouching ? 0.5f : 1f; //If crouching, halve moveSpeed.
        if (holdingItem && heldItemMass > 1f) { moveSpeedMult /= heldItemMass; } //If holding item, divide moveSpeed by mass of held item
        moveSpeed = baseMoveSpeed * moveSpeedMult;
        moveDir = inputDir * moveSpeed; //inputDir is the normalized direction, moveDir includes moveSpeed
        if (moveSpeedMult != speedMultLastFrame)
        {
            animator.SetFloat("Speed Multiplier", moveSpeedMult);
        }
        speedMultLastFrame = moveSpeedMult;
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.y); //Assign velocity and keep current y-component of velocity

        //Smooth rotation
        targetAngle = (Vector2.SignedAngle(inputDir, Vector2.up) + 360f) % 360f;
        smoothAngle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, targetAngle, ref smoothTurnVelocity, smoothTurnTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, smoothAngle, 0));
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputDir = ctx.ReadValue<Vector2>();
        if (ctx.started || ctx.canceled)
        {
            movePressed = ctx.started;
        }
        if (ctx.canceled)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!onGround) { return; }
        jumping = true;
        jumpPercent = 0f;
        jumpSpeed = Mathf.Sqrt(2f * (gravity/rb.mass) * jumpHeight*jumpHeightMult);
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z); //Change upward velocity

        animator.SetBool("Jumping", true);
        animator.SetTrigger("Jump");
    }

    void Jumping()
    {
        if (!jumping) { return; }
        if (rb.velocity.y > 0f)
        {
            jumpPercent = Mathf.InverseLerp(jumpSpeed, 0f, rb.velocity.y);
        }
        else if (!falling)
        {
            falling = true;
            animator.SetTrigger("Fall");
        }
        if (jumpPercent > 0.98f)
        {
            jumpPercent = 1f;
        }
        if (!onGround && !jumpingLeftGround)
        {
            jumpingLeftGround = true;
        }
        if (jumpingLeftGround && onGround)
        {
            jumpingLeftGround = false;
            jumping = false;
            falling = false;
            animator.SetBool("Jumping", false);
        }
        animator.SetFloat("Jump Percent", jumpPercent);
    }
    public void Throw()
    {
        throwing = true;
        moving = false;
        movementEnabled = false;
        Invoke("EndThrow", throwEndLag);
        transform.rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
        if (onGround)
        {
            var inputDir3 = new Vector3(inputDir.x, 0, inputDir.y);
            rb.velocity = Vector3.zero;
            rb.AddForce((moving?inputDir3:transform.forward)*throwMoveImpulse, ForceMode.Impulse);
        }
    }
    void EndThrow()
    {
        throwing = false;
        movementEnabled = true;
        animator.SetBool("Throwing", false);
    }
    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        //OnCrouchDown
        if (ctx.performed)
        {
            crouching = true;
            model.localScale = new Vector3(origScale.x * 1.2f, origScale.y * 0.5f, origScale.z * 1.2f);
            //model.localPosition = new Vector3(0, -0.5f, 0);
            //beanCollider.height = 1f;
            //beanCollider.center = new Vector3(0, -0.5f, 0);
        }
        //OnCrouchUp
        else if (ctx.canceled)
        {
            crouching = false;
            model.localScale = origScale;
            //model.localPosition = Vector3.zero;
            //beanCollider.height = 2f;
            //beanCollider.center = new Vector3(0, 0, 0);
        }
    }
}
