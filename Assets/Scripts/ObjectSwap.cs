using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject objectWithCollider;
    public GameObject objectWithoutCollider;
    public LightController lightController;

    private void Start()
    {
        // Optionally initialize the correct object state based on the light condition
        SwapObjectsBasedOnLight();
    }

    private void Update()
    {
        SwapObjectsBasedOnLight();
    }

    private void SwapObjectsBasedOnLight()
    {
        if (lightController.isLightOn)
        {
            objectWithCollider.SetActive(true);
            objectWithoutCollider.SetActive(false);
        }
        else
        {
            objectWithCollider.SetActive(false);
            objectWithoutCollider.SetActive(true);
        }
    }
}
