using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController _characterController;
    private InputActions _input;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    [Header("Movement")]
    [SerializeField] bool canMove = true;
    [SerializeField] float _movementMultiplier = 5.0f;
    [SerializeField] float _jumpForce = 2.0f;
    private float _movementX;
    private float _movementZ;
    [SerializeField] Vector3 _impactForce = Vector3.zero;


    [Header("Gravity")]
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] float _playerMass = 4.0f;

    void Awake()
    {
        _characterController = gameObject.AddComponent<CharacterController>();
        _input = new InputActions();
    }

    void FixedUpdate()
    {
        if (_impactForce.magnitude > 0.2)
            _characterController.Move(_impactForce * Time.deltaTime);
        // consumes the impact energy each cycle:
        _impactForce = Vector3.Lerp(_impactForce, Vector3.zero, 5 * Time.deltaTime);

        Vector3 move = new Vector3(_movementX, 0, _movementZ);
        _characterController.Move(move * _movementMultiplier * Time.deltaTime);

        _playerVelocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);


    }

    private void OnMove(InputValue movementValue)
    {
        _groundedPlayer = _characterController.isGrounded;

        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            //playerVelocity.y = 0f;
            _playerVelocity.y = _gravity * 0.1f;
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
        _groundedPlayer = _characterController.isGrounded;

        if (_groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpForce * -3.0f * _gravity);
        }
    }

    private void OnCrouch(InputValue value)
    {
        Debug.Log("Crouch");
    }

    public void ApplyMovementForce(Vector3 dir, float force)
    {
        Debug.Log($"Player got hit! dir: {dir}, force: {force}");

        dir.Normalize();
        if (dir.y < 0)
            dir.y = -dir.y; // reflect down force on the ground
        //_impactForce += dir.normalized * force / _playerMass;

        _impactForce += dir * (force / _playerMass);
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
