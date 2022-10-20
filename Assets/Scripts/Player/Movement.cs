using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed, jumpForce, smoothTurnTime;
    

    InputActions input;
    Rigidbody rb;
    GroundCheck groundCheck;
    Vector2 inputDir, moveDir;
    bool onGround = false;
    float targetAngle, smoothAngle, smoothTurnVelocity;
    void Start()
    {
        input = new InputActions();
        rb = GetComponent<Rigidbody>();
        groundCheck = transform.GetChild(4).GetComponent<GroundCheck>();
    }

    void Update()
    {
        onGround = groundCheck.onGround;
        Directional();
    }
    void Directional()
    {
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.y);
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
        moveDir = inputDir * moveSpeed;
    }

    void OnJump(InputValue value)
    {
        if (!onGround)
        {
            return;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCrouch(InputValue value)
    {
        Debug.Log("Crouch");
    }

    void OnCollisionEnter(Collision collision)
    {
        
    }


    //private void OnEnable()
    //{
    //    input.Ingame.Enable();
    //}
    //private void OnDisable()
    //{
    //    input.Ingame.Disable();
    //}
}
