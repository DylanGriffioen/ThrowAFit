using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    InputActions input;
    Rigidbody rb;
    GroundCheck groundCheck;
    Vector2 moveDir;
    bool onGround = false;

    void Start()
    {
        input = new InputActions();
        rb = GetComponent<Rigidbody>();
        groundCheck = transform.GetChild(4).GetComponent<GroundCheck>();
    }

    void Update()
    {
        onGround = groundCheck.onGround;
    }
    private void OnMove(InputValue movementValue)
    {
        moveDir = movementValue.Get<Vector2>();
        rb.AddForce(new Vector3(moveDir.x, 0, moveDir.y));
    }

    private void OnJump(InputValue value)
    {
        
    }

    private void OnCrouch(InputValue value)
    {
        Debug.Log("Crouch");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }


    private void OnEnable()
    {
        input.Ingame.Enable();
    }
    private void OnDisable()
    {
        input.Ingame.Disable();
    }
}
