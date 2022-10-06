using UnityEngine;
using UnityEngine.InputSystem;

/**
 *  (DD/MM/YY - 06.10.2022) 
 *  @Janek Tuisk
 */

/** TODO
 * Crouch
 * Need to play with float values to find best for our game
 */

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementX;
    [SerializeField] private float movementZ;
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] bool canMove = true;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float gravityForce = -9.81f;

    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        playerControls = new PlayerControls();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0f, movementZ);
        characterController.Move(movement * movementSpeed * Time.deltaTime);

        playerVelocity.y += gravityForce * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }


    private void OnMove(InputValue movementValue)
    {
        groundedPlayer = characterController.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            //playerVelocity.y = 0f;
            playerVelocity.y = gravityForce * 0.1f;
        }

        if (canMove)
        {
            Vector3 movementVector = movementValue.Get<Vector2>();

            movementX = movementVector.x;
            movementZ = movementVector.y;
        }
    }

    private void OnJump(InputValue movementValue)
    {
        groundedPlayer = characterController.isGrounded;

        if (groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityForce);
        }
    }

    /** (https://learn.unity.com/tutorial/taking-advantage-of-the-input-system-scripting-api?uv=2020.1&projectId=5fc93d81edbc2a137af402b7#5fcad3efedbc2a0020f781e1)
     * For the Input System commands to work, you will need to add both the OnEnable() and OnDisable() methods next.
     * Because the Input System is modular, you will be able to swap between different input assets or Control Schemes in the future.
     * */
    private void OnEnable()
    {
        playerControls.Player.Enable();
    }
    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
}
