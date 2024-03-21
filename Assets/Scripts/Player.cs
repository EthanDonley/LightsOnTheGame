using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 7f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    void Start()
    {
        bulby = GetComponent<Rigidbody2D>();
        bulbySprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Handle player movement
        HandleMovement();

        // Update coyote time counter
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer Stuff
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle player jumping
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }
        if (Input.GetButtonUp("Jump") && bulby.velocity.y > 0f)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.65f);
            coyoteTimeCounter = 0;
        }

        // Check for wall sliding
        if (isTouchingWall && !IsGrounded() && bulby.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Apply wall sliding effect
        if (isWallSliding)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, -wallSlideSpeed);
        }
    }

    void HandleMovement()
    {
        var userInput = Input.GetAxis("Horizontal");
        float moveHorizontal = userInput * speed;

        // Flip the sprite
        if (moveHorizontal < 0)
        {
            bulbySprite.flipX = true;
        }
        else if (moveHorizontal > 0)
        {
            bulbySprite.flipX = false;
        }

        // Apply movement
        bulby.velocity = new Vector2(moveHorizontal, bulby.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the wall
        if (collision.gameObject.layer == groundLayer)
        {
            isTouchingWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits the wall
        if (collision.gameObject.layer == groundLayer)
        {
            isTouchingWall = false;
            isWallSliding = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Jump()
    {
        if (isWallSliding)
        {
            // Wall jump
            Vector2 wallJumpDirection = Vector2.zero;
            if (isTouchingWall)
            {
                // Jump away from the wall
                wallJumpDirection = Vector2.up;
                if (bulby.velocity.x < 0)
                {
                    wallJumpDirection += Vector2.right;
                }
                else
                {
                    wallJumpDirection += Vector2.left;
                }
            }
            bulby.velocity = wallJumpDirection.normalized * wallJumpForce;
        }
        else
        {
            // Regular jump
            bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
        }
    }
}