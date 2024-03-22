using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoFilterColorChange : MonoBehaviour
{
    public TextMeshPro textComponent; // Reference to the GameObject to change color
    public LightController light; // Reference to the LightManager class

    // Reference to the Renderer component

    void Update()
    {
        // Check the boolean value from LightManager and set object color accordingly
        if (light.isLightOn)
        {
            textComponent.color = Color.black; // Set object color to white if light is on
        }
        else
        {
            textComponent.color = Color.white; // Set object color to black if light is off
        }
    }

}
