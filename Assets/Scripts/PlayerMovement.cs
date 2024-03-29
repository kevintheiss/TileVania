using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveInput;
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    CapsuleCollider2D playerBodyCollider;
    BoxCollider2D playerFeetCollider;

    float startingGravityScale;
    bool isAlive = true;

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        startingGravityScale = playerRigidBody.gravityScale;
    }

    void Update()
    {
        if(!isAlive)
        {
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if(value.isPressed)
        {
            playerRigidBody.velocity += new Vector2(0f, jumpSpeed);
            //Debug.Log(jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if(value.isPressed)
        {
            Instantiate(bullet, gun.position, transform.rotation);
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
        if(!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
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

    void Die()
    { 
        if(playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            playerAnimator.SetTrigger("Dying");
            playerRigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
