using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private bool isGrounded;
    private float coyoteTime = 0.2f;
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
        if (isGrounded)
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
    }

    void HandleMovement()
    {
        var userInput = Input.GetAxis("Horizontal");
        float moveHorizontal = userInput * speed;
        if (!isGrounded)
        {
            moveHorizontal *= 0.8f;
        }
        bulby.velocity = new Vector2(moveHorizontal, bulby.velocity.y);

        // Flip the sprite
        if (moveHorizontal < 0)
        {
            bulbySprite.flipX = true;
        }
        else if (moveHorizontal > 0)
        {
            bulbySprite.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        Collider2D[] collidersAbove = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Ground"));

        foreach (Collider2D collider in collidersAbove)
        {
            if (collider.gameObject != gameObject) // Ignore collisions with the player itself
            {
                // If there's a collider above the player, exit the method and don't perform the jump
                return;
            }
        }

        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
    }
}