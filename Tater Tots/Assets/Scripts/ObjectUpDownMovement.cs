using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDirection
{
    Upwards,
    Downwards
}

public class ObjectUpDownMovement : MonoBehaviour
{
    public float speed = 2f; // Speed of the up and down movement
    public float maxHeight = 5f; // Maximum height
    public float minHeight = 1f; // Minimum height
    public bool loopMovement = true; // Whether the movement should loop or not
    public MovementDirection direction = MovementDirection.Upwards; // Movement direction

    private int directionMultiplier; // 1 for up, -1 for down

    void Start()
    {
        directionMultiplier = direction == MovementDirection.Upwards ? 1 : -1; // Set direction multiplier based on selected direction
    }

    void Update()
    {
        // Calculate the object's next position
        Vector3 nextPosition = transform.position + Vector3.up * speed * Time.deltaTime * directionMultiplier;

        // Check if the object reached its maximum or minimum height, change direction if needed
        if (nextPosition.y >= maxHeight)
        {
            nextPosition.y = maxHeight;
            directionMultiplier = -1;
        }
        else if (nextPosition.y <= minHeight)
        {
            nextPosition.y = minHeight;
            directionMultiplier = 1;
        }

        // Update the object's position
        transform.position = nextPosition;

        // If not looping and at max or min height, stop movement
        if (!loopMovement && (nextPosition.y >= maxHeight || nextPosition.y <= minHeight))
        {
            enabled = false;
        }
    }
}
