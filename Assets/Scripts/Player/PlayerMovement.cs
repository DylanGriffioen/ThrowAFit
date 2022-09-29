using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    /**
     *  Adapted from the version (DD/MM/YY - 29.09.2022)  https://docs.unity3d.com/ScriptReference/CharacterController.Move.html 
     *  @Janek Tuisk
     */

    /** TODO
     * Need to play with float values to find best for our game
     *  Something is wrong with jumping. Doesn't always regonize boolean grounderPlayer or doesn't take input. 
     */

    [SerializeField] bool canMove = true;
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float gravityForce = -9.81f;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private void Start()
    {
        characterController = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        groundedPlayer = characterController.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (canMove)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            characterController.Move(move * Time.deltaTime * movementSpeed);


            if(move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityForce);
            }

            playerVelocity.y += gravityForce * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }
}
