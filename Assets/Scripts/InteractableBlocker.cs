using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBlocker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the tag "Crate"
        if (other.CompareTag("Interactable"))
        {
            // Perform actions to stop the crate (e.g., applying friction or stopping its movement)
            Rigidbody2D crateRb2d = other.GetComponent<Rigidbody2D>();
            if (crateRb2d != null)
            {
                // Apply friction or stop the crate's movement
                crateRb2d.velocity = Vector2.zero;
                crateRb2d.angularVelocity = 0f;
            }
        }
    }
}
