using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum PlayerControl
    {
        Player1,
        Player2
    }

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    [Header("Control Settings")]
    public PlayerControl control = PlayerControl.Player1;

    [Header("Car Settings")]
    public float maxAcceleration = 20f;
    public float brakeAcceleration = 5000f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 25.0f;
    public float speedMultiplier = 1f; // Normal speed multiplier
    public Vector3 _centerOfMass;
    public List<Wheel> wheels;

    [Header("Turbo Settings")]
    public float turboMultiplier = 2f; // Boost multiplier
    public bool isTurboActive = false;

    float moveInput;
    float steerInput;
    bool isBraking;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

        carRb.drag = 0.3f;
        carRb.angularDrag = 2.0f;
        carRb.mass = 1200f;

        foreach (var wheel in wheels)
        {
            WheelFrictionCurve sideways = wheel.wheelCollider.sidewaysFriction;
            sideways.extremumSlip = 0.2f;
            sideways.extremumValue = 1f;
            sideways.asymptoteSlip = 0.5f;
            sideways.asymptoteValue = 0.75f;
            sideways.stiffness = 3.0f;
            wheel.wheelCollider.sidewaysFriction = sideways;
        }
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        // WheelEffects(); // 🔇 Still disabled
    }

    void LateUpdate()
    {
        HandleMovement(); // Merged driving + braking + turbo logic
        Steer();
    }

    void GetInputs()
    {
        if (control == PlayerControl.Player1)
        {
            moveInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
            steerInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            isBraking = Input.GetKey(KeyCode.Q);
            isTurboActive = Input.GetKey(KeyCode.LeftShift); // 🚀 Player 1 Turbo
        }
        else if (control == PlayerControl.Player2)
        {
            moveInput = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
            steerInput = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
            isBraking = Input.GetKey(KeyCode.Space);
            isTurboActive = Input.GetKey(KeyCode.RightControl); // 🚀 Player 2 Turbo
        }
    }

    void HandleMovement()
    {
        float forwardVelocity = Vector3.Dot(transform.forward, carRb.velocity);

        // Apply turbo multiplier if active
        float turboBoost = isTurboActive ? turboMultiplier : 1f;
        float appliedTorque = moveInput * maxAcceleration * speedMultiplier * turboBoost * 150f;

        foreach (var wheel in wheels)
        {
            // Reset previous brake
            wheel.wheelCollider.brakeTorque = 0f;

            // Full brake
            if (isBraking)
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = brakeAcceleration;
            }
            // Engine braking when no input
            else if (moveInput == 0 && Mathf.Abs(forwardVelocity) > 0.5f)
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = brakeAcceleration * 0.5f;
            }
            // Normal drive or turbo drive
            else
            {
                wheel.wheelCollider.motorTorque = appliedTorque;
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }
    }

    void Steer()
    {
        float speed = carRb.velocity.magnitude;
        float speedFactor = Mathf.Clamp01(speed / 50f);
        float steerLimit = Mathf.Lerp(maxSteerAngle, maxSteerAngle * 0.25f, speedFactor);
        float _steerAngle = steerInput * turnSensitivity * steerLimit;

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    /*
    // 🔇 Commented out for now
    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (isBraking && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded && carRb.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
        }
    }
    */
}
