using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Material light; // Reference to the material using the black and white shader
    public bool isLightOn;

    public bool IsLightOn
    {
        get { return isLightOn; }
    }   
    void Update()
    {
        // Check if space bar is pressed
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Toggle the inversion effect
            ToggleInversion(!isLightOn);
        }
    }

    void Start()
    {
        // Apply initial inversion state
        UpdateInversion();
    }

    public void ToggleGlobalInversion(bool value)
    {
        isLightOn = value;
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

    public void ToggleInversion(bool value)
    {
        isLightOn = value;
        UpdateInversion();
    }
}
