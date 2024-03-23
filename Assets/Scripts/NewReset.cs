using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewReset : MonoBehaviour
{
    public Player player; // Reference to the PlayerController script
    public Transform checkpoint; // Drag the checkpoint GameObject to this field in the Inspector
    public LightController light;


    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }
    private void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Call the ResetPlayerPosition function when "R" key is pressed
            Reset();
        }
    }

    public void Reset()
    {
        // Check if the playerController reference is not null and if the checkpoint is assigned
        light.ToggleInversion(false);
        Vector2 checkpointPosition = checkpoint.position;
        if (player != null && checkpoint != null)
        {
            // Set player's position to the checkpoint's position
            
            player.transform.position = new Vector2(checkpointPosition.x, checkpointPosition.y);

            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (var obj in interactables)
            {
                var resetBehavior = obj.GetComponent<ObjectResetBehavior>();
                if (resetBehavior != null)
                {
                    resetBehavior.MoveToInitialPosition();
                }
            }
        }
        else
        {
            Debug.LogError("Player or checkpoint not assigned in the inspector!");
        }
    }
}
