using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 7f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private bool isTouchingGround;
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
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.5f);
            coyoteTimeCounter = 0;
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
        // Check if the player collides with the ground layer
        if (collision.gameObject.layer == groundLayer)
        {
            // Check if the collision contact is from the top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.9f) // Assuming the normal.y value is close to 1 when the collision is from the top
                {
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
        }
    }

    private bool IsGrounded()
    {
        // Check if the player is touching the ground layer or the platform effector from the top
        return isTouchingGround ||
               Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }


    void Jump()
    {
        // Check if the player is already jumping with a velocity greater than or equal to the standard jumping velocity
        if (Mathf.Abs(bulby.velocity.y) >= jumpForce)
        {
            return; // Don't apply jump if already jumping with sufficient velocity
        }
        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
    }
}