using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed, jumpForce, smoothTurnTime;

    Rigidbody rb;
    Transform bean;
    GroundCheck groundCheck;
    CapsuleCollider beanCollider;
    InputActions input;


    Vector3 origScale;
    Vector2 inputDir, moveDir;
    bool onGround, crouching = false;
    float targetAngle, smoothAngle, smoothTurnVelocity, _moveSpeed;
    

    void Start()
    {
        input = new InputActions();
        rb = GetComponent<Rigidbody>();
        groundCheck = transform.GetChild(3).GetComponent<GroundCheck>();
        bean = transform.GetChild(0);
        origScale = bean.localScale;
        _moveSpeed = moveSpeed;
        beanCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        onGround = groundCheck.onGround; //Update onGround from groundcheck object's collider
        Directional();
    }
    void Directional()
    {
        _moveSpeed = crouching ? moveSpeed / 2f : moveSpeed; //If crouching, halve moveSpeed.
        //_moveSpeed is editable in code while being able to adjust default moveSpeed in editor.
        moveDir = inputDir * _moveSpeed; //inputDir is the normalized direction, moveDir includes moveSpeed
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.y); //Assign velocity and keep current y-component of velocity

        //Smooth rotation
        if (inputDir.magnitude > 0.125f)
        {
            targetAngle = (Vector2.SignedAngle(inputDir, Vector2.up) + 360f) % 360f;
            smoothAngle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, targetAngle, ref smoothTurnVelocity, smoothTurnTime);
            transform.rotation = Quaternion.Euler(new Vector3(0, smoothAngle, 0));
        }
    }
    void OnMove(InputValue movementValue)
    {
        inputDir = movementValue.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!onGround)
        {
            return;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Add upward impulse
    }

    void OnCrouch(InputValue value)
    {
        //OnCrouchDown
        if (value.isPressed)
        {
            crouching = true;
            bean.localScale = new Vector3(origScale.x * 1.3f, origScale.y * 0.5f, origScale.z * 1.3f);
            bean.localPosition = new Vector3(0, -0.5f, 0);
            beanCollider.height = 1f;
            beanCollider.center = new Vector3(0, -0.5f, 0);
        }
        //OnCrouchUp
        else
        {
            crouching = false;
            bean.localScale = origScale;
            bean.localPosition = Vector3.zero;
            beanCollider.height = 2f;
            beanCollider.center = new Vector3(0, 0, 0);
        }
    }
}
