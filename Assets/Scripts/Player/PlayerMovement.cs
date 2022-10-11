using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController characterController;
    private InputActions _input;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Movement")]
    [SerializeField] bool canMove = true;
    [SerializeField] float _movementMultiplier = 5.0f;
    [SerializeField] float _jumpForce = 2.0f;
    private float _movementX;
    private float _movementZ;

    [Header("Gravity")]
    [SerializeField] float _gravity = -9.81f;

    void Awake()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        _input = new InputActions();
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(_movementX, 0, _movementZ);
        characterController.Move(move * _movementMultiplier * Time.deltaTime);

        playerVelocity.y += _gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void OnMove(InputValue movementValue)
    {
        groundedPlayer = characterController.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            //playerVelocity.y = 0f;
            playerVelocity.y = _gravity * 0.1f;
        }
        if (canMove)
        {
            Vector3 movementVector = movementValue.Get<Vector2>();
            _movementX = movementVector.x;
            _movementZ = movementVector.y;

            Vector3 move = new Vector3(_movementX, 0, _movementZ);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }
        }
    }

    private void OnJump(InputValue value)
    {
        groundedPlayer = characterController.isGrounded;

        if (groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(_jumpForce * -3.0f * _gravity);
        }
    }

    private void OnCrouch(InputValue value)
    {
        Debug.Log("Crouch");
    }


    private void OnEnable()
    {
        _input.Ingame.Enable();
    }
    private void OnDisable()
    {
        _input.Ingame.Disable();
    }
}
