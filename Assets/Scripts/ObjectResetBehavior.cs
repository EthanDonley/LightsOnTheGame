using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectResetBehavior : MonoBehaviour
{
    private Vector2 initialPosition;

    private void Start()
    {
        // Record the initial position on Start
        RecordInitialPosition();
    }

    private void RecordInitialPosition()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
    }

    public void MoveToInitialPosition()
    {
        // Move the object back to its initial position
        transform.position = initialPosition;
    }
}