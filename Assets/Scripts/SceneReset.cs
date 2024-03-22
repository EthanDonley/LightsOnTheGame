using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    public Player player;
    private Transform checkpoint;

    void Update()
    {
        // Check if the 'R' key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Save the current checkpoint
            SaveCheckpoint();

            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void SaveCheckpoint()
    {
        // Check if the player reference is not null and if the checkpoint is set
        if (player != null && player.checkpoint != null)
        {
            // Save the checkpoint
            checkpoint = player.checkpoint;
        }
    }

    // Called when the scene is loaded
    void OnEnable()
    {
        // Restore the checkpoint after the scene is loaded
        RestoreCheckpoint();
    }

    void RestoreCheckpoint()
    {
        // Check if the player reference is not null and if a checkpoint is saved
        if (player != null && checkpoint != null)
        {
            // Restore the checkpoint
            player.checkpoint = checkpoint;
        }
    }
}