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
    private int groundCount; // Keeps track of the number of ground contacts

    void Start()
    {
        bulby = GetComponent<Rigidbody2D>();
        bulbySprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Handle player movement
        HandleMovement();

        // Handle player jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void HandleMovement()
    {
        var userInput = Input.GetAxis("Horizontal");
        float moveHorizontal = userInput * speed;
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
            groundCount++; // Increment ground contact count
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundCount--; // Decrement ground contact count
            if (groundCount == 0) // If no more ground contacts
            {
                isGrounded = false;
            }
        }
    }

    void Jump()
    {
        // Perform the jump only when grounded
        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
    }
}