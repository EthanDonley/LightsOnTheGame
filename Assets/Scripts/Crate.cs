using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public float pushForce = 1f; //This is primarily used when lights are off
    private Rigidbody2D rb;
    public float exaggerationFactor = 2f; //This is mostly for calculations for box speed
    public float maxSpeed = 5f;
    public float maxSpeedDark = 8f;

    public LightController lightController;

    public PhysicsMaterial2D lightOnPhysicsMaterial;

    private bool isGoingDown = false;

    public LayerMask groundLayer;
    float BoxHeight = 0.2f;
    float FeelDistance = 0.1f;
    private float[] RayXPositions;
    private bool previousLightState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.drag = 7f;
        RayXPositions = new float[] { -0.15f, 0.0f, 0.15f, };
        previousLightState = lightController.isLightOn;
    }

    private void FixedUpdate()
    {
        if (lightController.isLightOn != previousLightState)
        {
            if (!lightController.isLightOn)
            {
                // When lights turn off, zero out horizontal velocity
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            previousLightState = lightController.isLightOn;
        }

        // Adjust velocity based on light state and speed limits
        if (rb.velocity.magnitude > (lightController.isLightOn ? maxSpeed : maxSpeedDark))
        {
            rb.velocity = rb.velocity.normalized * (lightController.isLightOn ? maxSpeed : maxSpeedDark);
        }

        // Prevent downward movement when lights are ON
        if (lightController.isLightOn)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pushForce = 3f;
            rb.gravityScale = 0f;
            rb.drag = 0f;
            GetComponent<Collider2D>().sharedMaterial = lightOnPhysicsMaterial;
        }
        else if (!lightController.isLightOn)
        {
            // Restore normal physics when the lights are off
            pushForce = 1f;
            rb.gravityScale = 4.5f;
            rb.drag = 7f;
            rb.mass = 9f;
            GetComponent<Collider2D>().sharedMaterial = null;
        }


        //Physics calculations are best suited for FixedUpdate()
    }

    void PreventDownwardMovement()
    {
        // Temporarily set gravityScale to 0 to prevent gravity effects
        rb.gravityScale = 0;

        // Check if moving downwards or if the player is on top, applying a slight upward force to counteract the player's weight
        // This requires a more sophisticated check to determine if the player is indeed on top of the crate
        if (rb.velocity.y < 0 || IsPlayerOnTop())
        {
            // Apply an upward force just enough to counteract the downward pressure
            rb.AddForce(Vector2.up * CalculateRequiredUpwardForce(), ForceMode2D.Force);
        }
    }

    float CalculateRequiredUpwardForce()
    {
        return 9.81f * rb.mass; // Placeholder calculation, adjust as needed
    }

    //We dont need this method for now
    bool IsPlayerOnTop()
    {
        Collider2D playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        RaycastHit2D hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, 0.1f);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            return true;
        }

      

        return false;
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {

        //When player touches the box and when lights are on
        if (collision.gameObject.CompareTag("Player") && lightController.IsLightOn)
        {
            Vector2 contactNormal = collision.contacts[0].normal;
            Vector2 direction = Vector2.zero;
            rb.mass = 100000;
            

            //Just some fancy math, speed should be primarily constant with the maxSpeed
            if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
            {
                direction.x = Mathf.Sign(collision.relativeVelocity.x); // Use relative velocity direction
            }
            else
            {
                direction.y = Mathf.Sign(contactNormal.y);
            }

            rb.velocity = direction * maxSpeed;

            if (lightController.IsLightOn)
            {
                rb.velocity *= exaggerationFactor;
            }
        }
        else
        {
            //Other Collisions
            Vector2 pushDirection = (collision.transform.position - transform.position).normalized; //Calculate direction between objects
            rb.mass = 9;
            //More Fancy Math
            float pushForceMagnitude = Mathf.Abs(Vector2.Dot(rb.velocity, pushDirection)) * pushForce;

            rb.AddForce(pushDirection * pushForceMagnitude, ForceMode2D.Impulse);

            //Also clamps the speed to the current max speed to prevent constant acceleration and improve consistency
            if (lightController.IsLightOn)
            {
                rb.AddForce(pushDirection * pushForceMagnitude * exaggerationFactor, ForceMode2D.Impulse);

                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            }

            //Check if the collided object is stationary like a wall and stops momentum entirely
            Rigidbody2D otherRigidbody = collision.collider.attachedRigidbody;
            if (otherRigidbody != null && otherRigidbody.bodyType == RigidbodyType2D.Static)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }



    //This shit is useless rn lol
    bool IsGrounded()
    {
        if (rb.velocity.y < 0.01f)
        {
            // Cast a bunch, left to right, looking for ground
            foreach (var xposition in RayXPositions)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    origin: transform.position + transform.right * xposition,
                    direction: Vector3.down,
                    distance: BoxHeight / 2 + FeelDistance,
                    layerMask: groundLayer);

                // Ignore collisions with own collider
                Physics2D.IgnoreCollision(hit.collider, GetComponent<Collider2D>(), true);

                if (hit.collider)
                {
                    return true;
                }
            }
        }
        return false;
    }

}