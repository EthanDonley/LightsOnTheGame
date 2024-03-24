using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCam; //We can attach this in the Unity Editor

    public float offset = 8f; //Basically when player enters a room they are mea

    private bool canOffsetPlayer = false; //Sometimes when respawning/dying player shifts 

    public Transform leftApproachCheckpoint; //Checkpoint if Player entered from left of the room
    public Transform rightApproachCheckpoint; //Checkpoint if Player entered from right of the room
    public Player player;
    public bool facingRight;


    public NewReset resetButton;


    private void Start()
    {
        StartCoroutine(DelayOffsetActivation());
    }


    IEnumerator DelayOffsetActivation()
    {
        yield return new WaitForSeconds(0.4f);
        canOffsetPlayer = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            
            Vector2 playerPos = other.transform.position;
            Vector2 roomPos = transform.position;
            if (canOffsetPlayer)
            {
                

                if (playerPos.x < roomPos.x)
                {
                    playerPos.x += offset;
                    player.checkpoint = leftApproachCheckpoint.transform;
                    player.UpdateCheckpoint(leftApproachCheckpoint);
                    SomeMethodToSetCheckpoint(leftApproachCheckpoint);
                    facingRight = false;
                }
                else
                {
                    playerPos.x -= offset;
                    player.checkpoint = rightApproachCheckpoint.transform;
                    player.UpdateCheckpoint(rightApproachCheckpoint);
                    SomeMethodToSetCheckpoint(rightApproachCheckpoint);
                    facingRight = true;
                }

                other.transform.position = playerPos;
            }
            virtualCam.SetActive(true);
        }
        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            
            virtualCam.SetActive(false);
            if (resetButton != null)
            {
                resetButton.ResetObjects();
            }
        }
    }

    public void SomeMethodToSetCheckpoint(Transform newCheckpoint)
    {
        resetButton.SetCheckpoint(newCheckpoint);
    }


    public void PlayerDied()
    {
        canOffsetPlayer = false;
        StartCoroutine(DelayOffsetActivation());
        player.FlipSprite(facingRight);
    }

}