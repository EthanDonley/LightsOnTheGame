using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCam;
    public float offset = 8f;
    private bool canOffsetPlayer = false;


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
            if (canOffsetPlayer)
            {
                Vector2 playerPos = other.transform.position;
                Vector2 roomPos = transform.position;

                if (playerPos.x < roomPos.x)
                {
                    playerPos.x += offset;
                }
                else
                {
                    playerPos.x -= offset;
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
}