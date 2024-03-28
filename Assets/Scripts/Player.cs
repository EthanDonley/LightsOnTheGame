using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxFallSpeed = -6f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    public Transform checkpoint;

    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private bool isTouchingGround;
    private bool bufferedHalfJump = false;
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.05f;
    private float jumpBufferCounter;
    private float[] RayXPositions;
    [SerializeField] private Collider2D groundCheckCollider;
    private bool isGrounded;
    private Vector2 movementInput;
    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool buffered = false;
    float PlayerHeight = 0.6f;


    public NewReset dead;

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
        RayXPositions = new float[] { -0.1f, 0.0f, 0.1f, };
    }

    void Update()
    {
        // Process inputs
        movementInput.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpReleased = true;
        }

        isGrounded = IsGrounded();
        

        // Update coyote time counter
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            animator.SetBool("isJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            animator.SetBool("isJumping", true);
        }

        // Handle animation flipping and running
        HandleAnimation();
    }

    void HandleAnimation()
    {
        if (movementInput.x < 0)
        {
            bulbySprite.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            bulbySprite.flipX = false;
        }
        animator.SetFloat("xVelocity", Mathf.Abs(bulby.velocity.x));
        animator.SetFloat("yVelocity", bulby.velocity.y);
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        float moveHorizontal = movementInput.x * speed;
        bulby.velocity = new Vector2(moveHorizontal, bulby.velocity.y);

        // Apply air dampening
        if (!isGrounded && moveHorizontal == 0)
        {
            bulby.velocity = new Vector2(bulby.velocity.x * 0.9f, bulby.velocity.y);
        }

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime; 
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle Jumping
        if (jumpPressed && (coyoteTimeCounter > 0f && jumpBufferCounter > 0f))
        {
            PerformJump();
        }

        if (jumpReleased && bulby.velocity.y > 0)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.5f);
        }

        // Clamp maximum fall speed
        if (bulby.velocity.y < maxFallSpeed)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, maxFallSpeed);
        }

        // Reset input flags
        jumpPressed = false;
        jumpReleased = false;

    }

    void PerformJump()
    {
        // Full jump if the jump button is held down
        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        animator.SetBool("isJumping", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pit"))
        {
            ResetToCheckpoint();
        }

        /*if (other.gameObject.CompareTag("Coin"))
        {
            playerCoins.IncrementNumCoins();
            playerCoins.RemoveCoin(other.gameObject);
            playerCoins.displayNumCoins();
        }*/
    }

    private void ResetToCheckpoint()
    {
        // Retrieve the ResetButton script and call its ResetPlayerPosition method
        NewReset resetButton = FindObjectOfType<NewReset>();
        Room room = FindObjectOfType<Room>();
        if (room != null)
        {

            room.PlayerDied();
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
        else
        {
            Debug.LogError("Room script not found in the scene!");
        }
        if (resetButton != null)
        {
            resetButton.Reset();
        }
        else
        {
            Debug.LogError("ResetButton script not found in the scene!");
        }
    }

    public void CallRoomReset() //This is meant to be public and called when doing Resetting specifically
    {
        Room room = FindObjectOfType<Room>();
        if (room != null)
        {
            room.PlayerDied();
        }
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

    bool IsGrounded()
    {
        return groundCheckCollider.IsTouchingLayers(groundLayer);
    }

    private Vector2 GetPlatformVelocity()
    {
        // Cast a ray downwards to check for a platform
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        if (hit.collider != null)
        {
            // If the player is standing on a platform, get its velocity
            Rigidbody2D platformRigidbody = hit.collider.GetComponent<Rigidbody2D>();
            if (platformRigidbody != null)
            {
                return platformRigidbody.velocity;
            }
        }

        // If the player is not standing on a platform, return zero
        return Vector2.zero;
    }

    public void UpdateCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }


    public void FlipSprite(bool facingRight)
    {
        // Flip the player's sprite horizontally based on facingRight flag
        bulbySprite.flipX = !facingRight;
    }

}