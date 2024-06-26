using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Player : MonoBehaviour
{

    public ParticleSystem dust;

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
    private bool isFacingRight = true;
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
        float scaleX = 0.15f;
        float scaleY = 0.15f;
        float scaleZ = 0.15f; // Assuming Z scale is also 0.15, adjust if different

        if (movementInput.x < 0)
        {
            // Flip the entire GameObject to face left, preserving scale
            transform.localScale = new Vector3(-scaleX, scaleY, scaleZ);
            isFacingRight = false;
        }
        else if (movementInput.x > 0)
        {
            // Flip the entire GameObject to face right, preserving scale
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            isFacingRight = true;

        }
        animator.SetFloat("xVelocity", Mathf.Abs(bulby.velocity.x));
        animator.SetFloat("yVelocity", bulby.velocity.y);
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        float moveHorizontal = movementInput.x * speed;

        bool leftPressed = movementInput.x < 0;
        bool rightPressed = movementInput.x > 0;

        // Cancel out horizontal movement if both left and right are being pressed
        if (leftPressed && rightPressed)
        {
            bulby.velocity = new Vector2(0, bulby.velocity.y);
        }
        else
        {
            bulby.velocity = new Vector2(moveHorizontal, bulby.velocity.y);
        }

        // Apply air dampening
        if (!isGrounded && moveHorizontal == 0)
        {
            bulby.velocity = new Vector2(bulby.velocity.x * 0.894f, bulby.velocity.y);
        }

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime; 
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        //Handle Jumping
        if (jumpPressed && (coyoteTimeCounter > 0f && jumpBufferCounter > 0f))
        {
            PerformJump();
        }

        if (jumpReleased && bulby.velocity.y > 0)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, bulby.velocity.y * 0.5f);
        }

        //Clamp maximum fall speed
        if (bulby.velocity.y < maxFallSpeed)
        {
            bulby.velocity = new Vector2(bulby.velocity.x, maxFallSpeed);
        }

        //Reset input flags
        jumpPressed = false;
        jumpReleased = false;

    }

    void PerformJump()
    {
        // Full jump if the jump button is held down
        bulby.velocity = new Vector2(bulby.velocity.x, jumpForce);
        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        CreateDust();
        animator.SetBool("isJumping", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pit") || other.CompareTag("Swappable"))
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

    public bool checkDirectionFacing()
    {
        return isFacingRight;
    }
    bool IsGrounded()
    {
        Vector2 position = transform.position;
        float distance = 0.387f; // Adjust based on the expected distance to the ground.
        float width = 0.0735f; // Half the width of the player's collider.
        int rayCount = 3; // Total number of rays to cast.
        float maxGroundAngle = 45; // Maximum angle to consider a surface as ground.
        float verticalVelocityThreshold = -0.1f; // Velocity threshold for determining "falling" state.
        float leftOffset = 0.24f;

        bool isGrounded = false;
        float raySpacing = ((width * 2) + leftOffset) / (rayCount - 1);

        if (bulby.velocity.y > verticalVelocityThreshold)
        {
            // Check directly below the player with a center ray.
            RaycastHit2D centerHit = Physics2D.Raycast(position + Vector2.left * leftOffset / 2, Vector2.down, distance, groundLayer);
            if (centerHit.collider != null && IsValidGround(centerHit, maxGroundAngle))
            {
                return true; // Early return if center ray detects valid ground.
            }

            // Cast rays across the player's width for comprehensive ground checking.
            for (int i = 0; i < rayCount; i++)
            {
                Vector2 rayOrigin = position + Vector2.left * (width + leftOffset) + Vector2.right * raySpacing * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distance, groundLayer);
                if (hit.collider != null && IsValidGround(hit, maxGroundAngle))
                {
                    return true; // Return true as soon as any ray detects valid ground.
                }
            }
        }

        return isGrounded; // Return the final grounded state.
    }

    // This method now takes maxGroundAngle as a parameter to validate the ground hit.
    bool IsValidGround(RaycastHit2D hit, float maxGroundAngle)
    {
        float angle = Vector2.Angle(hit.normal, Vector2.up);
        return angle <= maxGroundAngle && hit.point.y < transform.position.y;
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

    void CreateDust()
    {
        dust.Play();
    }
}