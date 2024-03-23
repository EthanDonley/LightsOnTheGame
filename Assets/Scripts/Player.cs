using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 7f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    public Transform checkpoint;

    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private bool isTouchingGround;
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    Animator animator;

    
    void Start()
    {
        bulby = GetComponent<Rigidbody2D>();
        bulbySprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (checkpoint != null)
        {
            Vector2 checkpointPosition = checkpoint.position;
            transform.position = new Vector2(checkpointPosition.x, checkpointPosition.y);
        }
    }

    void Update()
    {
        // Handle player movement
        HandleMovement();

        // Update coyote time counter
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            animator.SetBool("isJumping", false);
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
            animator.SetBool("isJumping", true);
            Jump();
            jumpBufferCounter = 0f;

        }
        if (Input.GetButtonUp("Jump") && bulby.velocity.y > 0f)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.5f);
            coyoteTimeCounter = 0;
        }

    }

    void HandleMovement()
    {
        var userInput = Input.GetAxis("Horizontal");
        float moveHorizontal = userInput * speed;

        // Apply movement
        bulby.velocity = new Vector2(moveHorizontal, bulby.velocity.y);

        // Apply air dampening if the player is in the air and not actively moving horizontally
        if (!isTouchingGround && moveHorizontal == 0)
        {
            // Apply a dampening force to reduce horizontal velocity gradually
            bulby.velocity = new Vector2(bulby.velocity.x * 0.95f, bulby.velocity.y);
        }

        // Flip the sprite
        if (moveHorizontal < 0)
        {
            bulbySprite.flipX = true;
        }
        else if (moveHorizontal > 0)
        {
            bulbySprite.flipX = false;
        }
        animator.SetFloat("xVelocity", Math.Abs(bulby.velocity.x));
        animator.SetFloat("yVelocity", bulby.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground layer
        if (collision.gameObject.layer == groundLayer)
        {
            // Check if the collision contact is from the top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.9f) // Assuming the normal.y value is close to 1 when the collision is from the top
                {
                    animator.SetBool("isJumping", false);
                    isTouchingGround = true;
                    break; // Exit the loop once a valid contact from the top is found
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits the wall
        if (collision.gameObject.layer == groundLayer)
        {
            isTouchingGround = false;
            animator.SetBool("isJumping", true);
        }
    }

    private bool IsGrounded()
    {
        float raycastLength = 0.1f; // Adjust the length of the raycast

        // Cast a ray downward from the player's feet position
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, raycastLength, groundLayer);

        // If the ray hits the ground layer, the player is considered grounded
        if (hit.collider != null)
        {
            animator.SetBool("isJumping", false);
            return true;
        }

        // If no ground is detected below the player, set isJumping to true
        animator.SetBool("isJumping", true);
        return false;
    }


    void Jump()
    {
        // Check if the player is already jumping with a velocity greater than or equal to the standard jumping velocity
        if (Mathf.Abs(bulby.velocity.y) >= jumpForce)
        {
            return; // Don't apply jump if already jumping with sufficient velocity
        }
        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);

        animator.SetBool("isJumping", true);
    }

    public void UpdateCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }
}