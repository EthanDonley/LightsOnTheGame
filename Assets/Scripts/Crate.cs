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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.drag = 7f;
    }
    void Update()
    {
        if (lightController.IsLightOn)
        {
            //Change gravity + make box never stop moving in light mode until it hits something
            pushForce = 3f;
            rb.gravityScale = 0f;
            rb.drag = 0f;

            //These physics for lights on are meant to stop the box from bouncing on walls and make it seem slide-ier
            GetComponent<Collider2D>().sharedMaterial = lightOnPhysicsMaterial;
        }
        else
        {
            pushForce = 1f;
            rb.gravityScale = 5f;
            rb.drag = 7f; 

            /*The idea here is that when the lights are off, the physics 2d material is turned off as it 
             would make the box without light friction-less causing problems*/

            GetComponent<Collider2D>().sharedMaterial = null;
        }


        //Check if the crate is moving downward
        isGoingDown = rb.velocity.y < 0;

        /*
        if (IsPlayerOnTop())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }*/     

        if (isGoingDown && !lightController.IsLightOn)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed && lightController.IsLightOn)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else if (rb.velocity.magnitude > maxSpeedDark && !lightController.IsLightOn)
        {
            rb.velocity = rb.velocity.normalized * maxSpeedDark;
        }

        //Physics calculations are best suited for FixedUpdate()
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

            /*if (lightController.IsLightOn)
            {
                rb.velocity *= exaggerationFactor;
            }*/
        }
        else
        {
            //Other Collisions
            Vector2 pushDirection = (collision.transform.position - transform.position).normalized; //Calculate direction between objects
            
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

}