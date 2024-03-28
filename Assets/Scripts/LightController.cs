using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Material light; // Reference to the material using the black and white shader
    public bool isLightOn;
    public float transitionDuration = 1.0f; // Duration of the transition effect
    public float pauseDuration = 0.5f; // Duration to pause the game before starting the transition
    public bool isTransitioning = false; // To prevent multiple transitions at once
    public float cooldown = 2f; // Cooldown duration in seconds
    private float lastToggleTime = -2f; // Time since the last toggle

    void Update()
    {
        // Check if Z key is pressed and ensure cooldown has passed
        if (Input.GetKeyDown(KeyCode.Z) && (Time.time - lastToggleTime >= cooldown) && !isTransitioning)
        {
            lastToggleTime = Time.time;
            StartCoroutine(FadeToBlackAndWhiteCoroutine(!isLightOn));
        }
    }

    IEnumerator FadeToBlackAndWhiteCoroutine(bool targetState)
    {
        isTransitioning = true;

        // Temporarily pause game logic
        Time.timeScale = 0;
        float startValue = light.GetFloat("_IsInverted");
        float targetValue = targetState ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime since timeScale is 0
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / transitionDuration);
            light.SetFloat("_IsInverted", newValue);
            yield return null; // Wait for real time, not game time
        }

        // Ensure the target value is set after the transition
        light.SetFloat("_IsInverted", targetValue);

        // Resume game logic
        Time.timeScale = 1;

        // Toggle the inversion state
        ToggleInversion(targetState);

        isTransitioning = false;
    }

    public bool IsLightOn
    {
        get { return isLightOn; }
    }
    public void ToggleInversion(bool value)
    {
        isLightOn = value;
        // Optionally, notify other components that the inversion has been toggled
        // Implement any notifications or updates to other components here
    }

    void Start()
    {
        // Apply initial inversion state
        UpdateInversion();
    }

    void UpdateInversion()
    {
        // Apply inversion effect globally if enabled
        if (light != null)
        {
            light.SetFloat("_IsInverted", isLightOn ? 1 : 0);
        }
    }
}