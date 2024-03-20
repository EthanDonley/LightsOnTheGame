using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Material light; // Reference to the material using the black and white shader
    public bool isLightOn;
    void Update()
    {
        // Check if space bar is pressed
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Toggle the inversion effect
            if (light != null)
            {
                // Toggle the _IsInverted property of the shader
                light.SetFloat("_IsInverted", light.GetFloat("_IsInverted") == 1 ? 0 : 1);
            }
        }
    }

    public void ToggleGlobalInversion(bool value)
    {
        isLightOn = value;

        // Apply inversion effect globally if enabled
        if (isLightOn)
        {
            if (light != null)
            {
                light.SetFloat("_IsInverted", 1);
            }
        }
        else
        {
            if (light != null)
            {
                light.SetFloat("_IsInverted", 0);
            }
        }
    }
}
