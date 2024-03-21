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
        rb.drag = 7f; // Add some linear drag to reduce sliding
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
            rb.gravityScale = 5f;
            rb.drag = 7f; // Adjust drag value as needed

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
        if (collision.gameObject.CompareTag("Player") && lightController.IsLightOn)
        {
            // Calculate the contact point between the crate and the player
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            collision.GetContacts(contacts);

            // Check if the collision occurs at the bottom of the crate
            foreach (ContactPoint2D contact in contacts)
            {
                if (contact.normal.y > 0.9f) // Assuming upward direction is (0, 1)
                {
                    // Apply force to push the crate directly upwards
                    rb.velocity = new Vector2(rb.velocity.x, pushForce);
                    return; // Exit the loop after applying force
                }
            }
        }

        // If the lights are off or the collision doesn't occur at the bottom, apply regular horizontal force
        // Calculate push direction based on player's position relative to the crate
        float pushDirection = Mathf.Sign(collision.transform.position.x - transform.position.x);

        // Apply force to push the crate horizontally
        rb.AddForce(Vector2.right * pushDirection * pushForce, ForceMode2D.Impulse);

        if (lightController.IsLightOn)
        {
            rb.AddForce(Vector2.right * pushDirection * pushForce * exaggerationFactor, ForceMode2D.Impulse);
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