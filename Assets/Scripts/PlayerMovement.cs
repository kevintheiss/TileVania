using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    Vector2 moveInput;
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    CapsuleCollider2D playerCapsuleCollider;
    float startingGravityScale;
  
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        startingGravityScale = playerRigidBody.gravityScale;
    }

    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if(!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if(value.isPressed)
        {
            playerRigidBody.velocity += new Vector2(0f, jumpSpeed);
            //Debug.Log(jumpSpeed);
        }
    }

    void Run()
    {
        bool isRunning = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;

        if (isRunning)
        {
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }
    }

    void FlipSprite()
    {
        bool isMovingHorizontal = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;

        if (isMovingHorizontal)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if(!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            playerRigidBody.gravityScale = startingGravityScale;
            playerAnimator.SetBool("isClimbing", false);
            return;
        }

        bool isClimbing = Mathf.Abs(playerRigidBody.velocity.y) > Mathf.Epsilon;
        Vector2 climbVelocity = new Vector2(playerRigidBody.velocity.x, moveInput.y * climbSpeed);
        playerRigidBody.velocity = climbVelocity;
        playerRigidBody.gravityScale = 0f;

        playerAnimator.SetBool("isClimbing", isClimbing);
    }
}
