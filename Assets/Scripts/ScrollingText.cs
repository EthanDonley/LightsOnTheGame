using System.Collections;
using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    public TextMeshPro displayText; // Assign your TMP Text component here
    public AudioSource scrollAudio; // Assign your AudioSource component here
    public string fullText = "This is the scrolling text that will be displayed with TextMeshPro."; // Text to scroll
    public float delay = 0.05f; // Delay between each character display
    private bool isPlayerClose = false;
    private Vector3 originalPosition;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            Debug.Log("Hi");
            StartCoroutine(ShowTextWithAnimation());
        }
    }

    IEnumerator ShowTextWithAnimation()
    {
        // Play scrolling sound
        scrollAudio.Play();

        displayText.text = ""; // Make sure the text is empty before starting
        Vector3 originalPosition = displayText.transform.position; // Save the original position

        // Scroll text with animation
        for (int i = 0; i <= fullText.Length; i++)
        {
            displayText.text = fullText.Substring(0, i);

            // Create a simple animation effect by moving the text's position slightly
            displayText.transform.position = originalPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            yield return new WaitForSeconds(delay);

            // Reset position to avoid drifting away
            displayText.transform.position = originalPosition;
        }

        // Stop scrolling sound when text finished
        scrollAudio.Stop();

        // Optionally, hide the text or destroy the object after some time
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the trigger zone
        if (other.CompareTag("Player") && isPlayerClose)
        {
            StopAllCoroutines(); // Stop the text if the player leaves early
            displayText.text = ""; // Clear the text
            displayText.transform.position = originalPosition; // Reset the position
            scrollAudio.Stop(); // Stop the audio if still playing
            isPlayerClose = false;
        }
    }
}