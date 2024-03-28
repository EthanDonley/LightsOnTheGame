using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoFilterColorChange : MonoBehaviour
{
    public TextMeshPro textComponent; // Reference to the GameObject to change color
    public LightController light;
    public float transitionDuration = 0.5f;
    private Color targetColor;
    public bool IsTransitioning { get; private set; } = false; // Reference to the LightManager class

    // Reference to the Renderer component
    void Start()
    {
        // Initialize the target color based on the initial state of the light
        targetColor = light.isLightOn ? Color.black : Color.white;
        textComponent.color = targetColor;
    }
    void Update()
    {
        // Update the target color based on the light state
        Color newTargetColor = light.isLightOn ? Color.black : Color.white;

        // If the light's transition state changes and it's not already transitioning the text color
        if (targetColor != newTargetColor && !light.isTransitioning)
        {
            targetColor = newTargetColor;
            StartCoroutine(TransitionColor(targetColor));
        }
    }

    IEnumerator TransitionColor(Color newColor)
    {
        Color startColor = textComponent.color;
        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            textComponent.color = Color.Lerp(startColor, newColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set after the transition
        textComponent.color = newColor;
    }

}
