using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCam;
    public float offset = 8f;
    private bool canOffsetPlayer = false;
    public Transform leftApproachCheckpoint;
    public Transform rightApproachCheckpoint;
    public Player player;

    public NewReset resetButton;


    private void Start()
    {
        StartCoroutine(DelayOffsetActivation());
    }

    IEnumerator DelayOffsetActivation()
    {
        yield return new WaitForSeconds(0.1f);
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
                }
                else
                {
                    playerPos.x -= offset;
                    player.checkpoint = rightApproachCheckpoint.transform;
                    player.UpdateCheckpoint(rightApproachCheckpoint);
                    SomeMethodToSetCheckpoint(rightApproachCheckpoint);
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
        }
    }

    public void SomeMethodToSetCheckpoint(Transform newCheckpoint)
    {
        resetButton.SetCheckpoint(newCheckpoint);
    }
}