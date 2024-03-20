using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public float pushForce = 10f; // Force applied when the crate is pushed
    private Rigidbody2D rb;
    public float exaggerationFactor = 2f;

    public LightController lightController;
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
            rb.gravityScale = 0f;
            rb.drag = 0f;
        }
        else
        {
            // Enable gravity and drag when lights are off
            rb.gravityScale = 7f;
            rb.drag = 10f; // Adjust drag value as needed
        }
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
}
