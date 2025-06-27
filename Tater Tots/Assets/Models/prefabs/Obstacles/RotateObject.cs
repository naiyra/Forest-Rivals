using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public enum RotationMode { Continuous, Pendulum }
    public RotationMode rotationMode = RotationMode.Continuous;

    public Vector3 rotationAxis = Vector3.up; // Default axis is Y-axis for continuous mode
    public float rotationSpeed = 10f; // Default rotation speed
    public bool clockwise = true; // Default direction is clockwise

    public Vector3 pendulumAxis = Vector3.right; // Default axis is X-axis for pendulum mode
    public float pendulumSpeed = 1f; // Speed of the pendulum motion
    public float pendulumAngle = 30f; // Max angle of the pendulum swing

    private float pendulumTime;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (rotationMode == RotationMode.Continuous)
        {
            RotateContinuously();
        }
        else if (rotationMode == RotationMode.Pendulum)
        {
            SwingLikePendulum();
        }
    }

    void RotateContinuously()
    {
        // Calculate the rotation direction
        float direction = clockwise ? 1f : -1f;

        // Apply the rotation
        transform.Rotate(rotationAxis, rotationSpeed * direction * Time.deltaTime, Space.Self);
    }

    void SwingLikePendulum()
    {
        // Increment time
        pendulumTime += Time.deltaTime * pendulumSpeed;

        // Calculate the angle for the pendulum swing
        float angle = Mathf.Sin(pendulumTime) * pendulumAngle;

        // Apply the rotation relative to the initial rotation
        transform.localRotation = initialRotation * Quaternion.Euler(pendulumAxis * angle);
    }
}
