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
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    private float[] RayXPositions;

    float PlayerHeight = 0.6f;
    float ToeFeelDistance = 0.1f;

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
            if (Input.GetButton("Jump"))
            {
                // Full jump if the jump button is held down
                bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
            }
            else
            {
                // Half jump if the jump button was tapped
                bulby.velocity = new Vector2(bulby.velocity.x, jumpForce * 0.7f);
                bufferedHalfJump = true;
            }
            jumpBufferCounter = 0f;

        }
        if (Input.GetButtonUp("Jump") && bulby.velocity.y > 0f && bufferedHalfJump == false)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.5f);
            coyoteTimeCounter = 0;
        }

        bufferedHalfJump = false;

    }

    void FixedUpdate()
    {

        // Clamp maximum fall speed
        if (bulby.velocity.y < maxFallSpeed)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, maxFallSpeed);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pit"))
        {
            ResetToCheckpoint();
        }
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

    private bool IsGrounded()
    {
        
        if (bulby.velocity.y < 0.01f)
        {
            //cast a bunch, left to right, looking for ground
            foreach (var xposition in RayXPositions)
            {
                Vector2 origin = transform.position + transform.right * xposition;
                RaycastHit2D hit = Physics2D.Raycast(
                    origin: origin,
                    direction: Vector3.down,
                    distance: PlayerHeight / 2 + ToeFeelDistance,
                    layerMask: groundLayer);

                if (hit.collider && Mathf.Abs(hit.normal.y) > 0.8f)
                {
                    animator.SetBool("isJumping", false);
                    return true;
                }

                Debug.DrawRay(origin, Vector3.down * (PlayerHeight / 2 + ToeFeelDistance), Color.green);
            }

           
        }

        animator.SetBool("isJumping", true);
        return false;


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