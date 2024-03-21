using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public float pushForce = 1f; // Force applied when the crate is pushed
    private Rigidbody2D rb;
    public float exaggerationFactor = 2f;

    public LightController lightController;

    public PhysicsMaterial2D lightOnPhysicsMaterial;

    // Boolean to track whether the crate is moving downward
    private bool isGoingDown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; // Set the rigidbody type to dynamic for physics interactions
        rb.drag = 10f; // Add some linear drag to reduce sliding
    }
    void Update()
    {
        if (lightController.IsLightOn)
        {
            // Disable gravity and drag when lights are on
            pushForce = 7f;
            rb.gravityScale = 0f;
            rb.drag = 0f;

            // Apply the physics material 2D to the crate's collider
            GetComponent<Collider2D>().sharedMaterial = lightOnPhysicsMaterial;
        }
        else
        {
            pushForce = 1f;
            rb.gravityScale = 7f;
            rb.drag = 10f; // Adjust drag value as needed

            // Remove the physics material 2D from the crate's collider
            GetComponent<Collider2D>().sharedMaterial = null;
        }

        // Check if the crate is moving downward
        isGoingDown = rb.velocity.y < 0;

        // Check if the player is on top of the crate and prevent horizontal movement
        if (IsPlayerOnTop())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    bool IsPlayerOnTop()
    {
        // Get the player's collider
        Collider2D playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();

        // Check if the player's collider is directly above the crate's collider
        RaycastHit2D hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, 0.1f);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the player's rigidbody component
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // Calculate push direction based on player's movement direction
            float pushDirection = Mathf.Sign(playerRb.velocity.x);

            // Apply force to push the crate
            rb.AddForce(Vector2.right * pushDirection * pushForce, ForceMode2D.Impulse);

            if (lightController.IsLightOn)
            {
                rb.AddForce(Vector2.right * pushDirection * pushForce * exaggerationFactor, ForceMode2D.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        // Check if the crate is moving downward and prevent horizontal movement
        if (isGoingDown)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }
}