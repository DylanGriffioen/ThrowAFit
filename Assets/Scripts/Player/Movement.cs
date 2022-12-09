using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Min(0.01f)] public float baseMoveSpeed, baseJumpHeigh, throwMoveImpulse, smoothTurnTime, crouchHeightMult, crouchWidthMult, ragdollThreshold;
    public GameObject ragdoll;

    PlaySFX sfxPlayer;
    Rigidbody rb;
    Transform model;
    [System.NonSerialized] public Transform itemSlot;
    Animator animator;
    Collider coll;
    GameObject newRagdoll;
    Transform newRagdollRoot;
    RagdollBehavior newRagdollBehavior;
    SkinnedMeshRenderer render;
    Follow followScript;
    Color color;

    Vector3 origScale, itemSlotOrigScale;
    Vector2 inputDir, moveDir;
    bool crouching, movePressed, moving, movingLastFrame, jumping, falling, jumpingLeftGround, velociraptor;
    [System.NonSerialized] public bool holdingItem, onGround, ragdolling, movementEnabled = true, hitStunned, grabbed;
    [System.NonSerialized] public float heldItemMass = 1f;
    float targetAngle, smoothAngle, smoothTurnVelocity, moveSpeed, moveSpeedMult, jumpSpeed, jumpHeightMult, 
        jumpPercent, speedMultLastFrame, gravity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity.magnitude;
        model = transform.GetChild(0);
        itemSlot = transform.GetChild(2);
        origScale = transform.localScale;
        itemSlotOrigScale = itemSlot.localScale;
        animator = model.GetComponent<Animator>();
        render = GetComponentInChildren<SkinnedMeshRenderer>();
        coll = GetComponent<CapsuleCollider>();
        followScript = GetComponent<Follow>();
        sfxPlayer = GameObject.Find("SFX").GetComponent<PlaySFX>();
    }
    void Start()
    {
        color = render.material.color;
    }
    void Update()
    {
        Initial();
        ItemSlotMove();
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
                animator.ResetTrigger("Fall");
            }
        }
        jumpHeightMult = 1f;
        if (holdingItem && heldItemMass >= 2f) { jumpHeightMult /= heldItemMass/2f; } 
        //If holding item, divide jump speed multiplier by mass of held item
        if (rb.velocity.magnitude > 0.5f)
        {
            velociraptor = true;
        }
        if (rb.velocity.magnitude < 0.01f)
        {
            velociraptor = false;
        }
        if (hitStunned && rb.velocity.magnitude < 0.2f && velociraptor)
        {
            hitStunned = false;
            movementEnabled = true;
        }
    }
    void ItemSlotMove()
    {
        if (moving && onGround)
        {
            var lerpValue = Mathf.Lerp(itemSlot.localPosition.z, 0.1f,0.2f);
            itemSlot.localPosition = new Vector3(itemSlot.localPosition.x, itemSlot.localPosition.y, lerpValue);
        }
        else
        {
            var lerpValue = Mathf.Lerp(itemSlot.localPosition.z, 0, 0.2f);
            itemSlot.localPosition = new Vector3(itemSlot.localPosition.x, itemSlot.localPosition.y, lerpValue);
        }
    }
    void Directional()
    {
        if (!moving) 
        {
            if (!hitStunned)
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
            return; 
        }

        //Directional movement
        moveSpeedMult = crouching ? 0.5f : 1f; //If crouching, halve moveSpeed.
        if (holdingItem && heldItemMass >= 2f) { moveSpeedMult /= heldItemMass/2f; } //If holding item, divide moveSpeed by mass of held item
        moveSpeedMult *= inputDir.magnitude;
        moveSpeed = baseMoveSpeed * moveSpeedMult;
        moveDir = inputDir.normalized * moveSpeed; //inputDir is the normalized direction, moveDir includes moveSpeed
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
        var value = ctx.ReadValue<Vector2>();
        if (value == Vector2.zero) 
        { 
            movePressed = false;  
            return; 
        }
        movePressed = true;
        inputDir = value;
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!onGround || !ctx.performed || !movementEnabled) { return; }
        jumping = true;
        jumpPercent = 0f;
        jumpSpeed = Mathf.Sqrt(2f * gravity * baseJumpHeigh*jumpHeightMult);
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z); //Change upward velocity

        animator.SetBool("Jumping", true);
        animator.SetTrigger("Jump");
    }
    void Jumping()
    {
        if (!jumping) { return; }
        if (rb.velocity.y > 0f && jumpPercent <= 0.93f)
        {
            jumpPercent = Mathf.InverseLerp(jumpSpeed, 0f, rb.velocity.y);
        }
        else if (!falling)
        {
            falling = true;
            animator.SetTrigger("Fall");
        }
        if (jumpPercent > 0.93f)
        {
            jumpPercent = Mathf.Lerp(jumpPercent, 1f, 0.2f);
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
    public void Throw(float throwEndLag)
    {
        moving = false;
        movementEnabled = false;
        Invoke("EndThrow", throwEndLag);
        transform.rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
        if (onGround)
        {
            var inputDir3 = new Vector3(inputDir.x, 0, inputDir.y);
            rb.velocity = Vector3.zero;
            rb.AddForce((moving?inputDir3:transform.forward)*throwMoveImpulse, ForceMode.VelocityChange);
        }
        sfxPlayer.Throw();
    }
    void EndThrow()
    {
        movementEnabled = true;
        animator.SetBool("Throwing", false);
    }
    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        //OnCrouchDown
        if (ctx.performed)
        {
            crouching = true;
            transform.localScale = new Vector3(origScale.x * crouchWidthMult, origScale.y * crouchHeightMult, origScale.z * crouchWidthMult);
            itemSlot.localScale = new Vector3(itemSlotOrigScale.x / crouchWidthMult, itemSlotOrigScale.y / crouchHeightMult, itemSlotOrigScale.z / crouchWidthMult);
            //model.localPosition = new Vector3(0, -0.5f, 0);
            //beanCollider.height = 1f;
            //beanCollider.center = new Vector3(0, -0.5f, 0);
        }
        //OnCrouchUp
        else if (ctx.canceled)
        {
            crouching = false;
            transform.localScale = origScale;
            itemSlot.localScale = itemSlotOrigScale;
            //model.localPosition = Vector3.zero;
            //beanCollider.height = 2f;
            //beanCollider.center = new Vector3(0, 0, 0);
        }
    }
    public void ObjectHitPlayer()
    {
        hitStunned = true;
        movementEnabled = false;
        Invoke("TurnOnRagdoll", 0.05f);
    }
    public void TurnOnRagdoll()
    {
        if (ragdolling || rb.velocity.magnitude < ragdollThreshold) { return; }
        ragdolling = true;
        render.enabled = false;
        rb.useGravity = false;
        var velocity = rb.velocity;
        rb.velocity = Vector3.zero;
        coll.isTrigger = true;
        movementEnabled = false;

        newRagdoll = Instantiate(ragdoll, transform.position, transform.rotation);
        newRagdollBehavior = newRagdoll.transform.GetChild(0).GetComponent<RagdollBehavior>();
        newRagdollBehavior.velocity = velocity;
        newRagdollBehavior.movementScript = GetComponent<Movement>();
        newRagdollBehavior.color = color;
        followScript.enabled = true;
        newRagdollRoot = newRagdoll.transform.GetChild(0).GetChild(0);
        followScript.target = newRagdollRoot;
    }
    public void TurnOffRagdoll()
    {
        if (!ragdolling) { return; }
        ragdolling = false;
        followScript.enabled = false;
        Destroy(newRagdoll);
        render.enabled = true;
        rb.useGravity = true;
        coll.isTrigger = false;
        movementEnabled = true;
    }
    public void OnRagdoll(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) { return; }
        if (!ragdolling)
        {
            TurnOnRagdoll();
            newRagdollBehavior.offWhenNotMoving = false;
        }
        else
        {
            TurnOffRagdoll();
        }
    }
    public void GetGrabbed(Transform itemSlot)
    {
        TurnOnRagdoll();
        newRagdollRoot.transform.parent = itemSlot;
        newRagdollBehavior.offWhenNotMoving = false;
    }
    public void GetThrown()
    {

    }
}
