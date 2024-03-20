using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody2D bulby;
    private SpriteRenderer bulbySprite;
    private float smoothInput;
    private bool isGrounded;

    void Start()
    {
        bulby = GetComponent<Rigidbody2D>();
        bulbySprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        var userInput = Input.GetAxis("Horizontal");
        smoothInput = Mathf.Lerp(smoothInput, userInput, Time.fixedDeltaTime * 10f);

        // Player Movement
        float moveHorizontal = smoothInput * speed;
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
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}