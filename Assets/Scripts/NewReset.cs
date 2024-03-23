using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewReset : MonoBehaviour
{
    public Player player; 
    public Transform checkpoint;
    public LightController light;

    private GameObject[] interactables;

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    private void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }
    private void Update()
    {
        //Reset button is currently assigned to R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void Reset()
    {
        //Check if the playerController reference is not null and if the checkpoint is assigned
        Vector2 checkpointPosition = checkpoint.position;
        if (player != null && checkpoint != null)
        {
            // Set player's position to the checkpoint's position
            Rigidbody2D playerRb2d = player.GetComponent<Rigidbody2D>();
            if (playerRb2d != null)
            {
                playerRb2d.velocity = Vector2.zero;
            }
            player.transform.position = new Vector2(checkpointPosition.x, checkpointPosition.y);
            player.CallRoomReset();
            


            foreach (var obj in interactables)
            {
                var resetBehavior = obj.GetComponent<ObjectResetBehavior>();
                if (resetBehavior != null)
                {
                    resetBehavior.MoveToInitialPosition();
                    Rigidbody2D rb2d = obj.GetComponent<Rigidbody2D>();
                    if (rb2d != null)
                    {
                        rb2d.velocity = Vector2.zero;
                    }
                }
            }

        }
        else
        {
            Debug.LogError("Player or checkpoint not assigned in the inspector!");
        }
    }

    private IEnumerator ResetInteractables()
    {
        // Wait for a small amount of time before resetting interactable objects
        yield return new WaitForSeconds(0.15f);

        // Reset all objects with the "Interactable" tag to their initial positions
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (var obj in interactables)
        {
            var resetBehavior = obj.GetComponent<ObjectResetBehavior>();
            if (resetBehavior != null)
            {
                resetBehavior.MoveToInitialPosition();
                Rigidbody2D rb2d = obj.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.velocity = Vector2.zero;
                }
            }
        }
    }

    //Resets only objects and does it with a timer to prevent instantly appearing objects in-between transitions
    public void ResetObjects()
    {
        StartCoroutine(ResetInteractables());
    }
}
