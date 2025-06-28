using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{

    private bool isStunned = false;
    private float stunDuration = 1.0f;
    private float stunTimer = 0f;



    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private float groundedCheckDelay = 0.2f;
    private float groundedCheckTimer;






    public enum ControlMode
    {
        Player1,
        Player2,
        Buttons
    };

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
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;
    bool isBraking;
    private bool isAirborne;  // Flag for checking if the car is airborne

    private Rigidbody carRb;
    private float boostTimer = 0f;
    private float speedBoostMultiplier = 1f;
    public int lastPassedWaypointIndex = -1; // Will be set by TrackPositionManager

    public Transform[] trackWaypoints; // Assign these from TrackPositionManager when the race starts

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;


    }

    void Update()
    {
        TrackLastValidPosition();
        CheckResetInput();

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
            return; // Skip inputs while stunned
        }


        GetInputs();
        AnimateWheels();



    }



    void TrackLastValidPosition()
    {
        groundedCheckTimer -= Time.deltaTime;

        if (groundedCheckTimer <= 0f)
        {
            if (IsGrounded())
            {
                lastValidPosition = transform.position;
                lastValidRotation = transform.rotation;
            }

            groundedCheckTimer = groundedCheckDelay;
        }
    }

    bool IsGrounded()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider.isGrounded)
                return true;
        }
        return false;
    }

    void CheckResetInput()
    {
        bool resetRequested = false;

        if (control == ControlMode.Player1 && Input.GetKeyDown(KeyCode.R))
            ResetCarToLastWaypoint();
        else if (control == ControlMode.Player2 && Input.GetKeyDown(KeyCode.L))  // ← NEW button for Player2
            ResetCarToLastWaypoint();


        if (resetRequested)
        {
            ResetCarToLastWaypoint();
        }
    }

    void ResetCarToLastWaypoint()
    {
        if (trackWaypoints == null || lastPassedWaypointIndex < 0 || lastPassedWaypointIndex >= trackWaypoints.Length)
        {
            Debug.LogWarning("Waypoint data invalid. Using fallback position.");
            transform.position = lastValidPosition + Vector3.up * 1f;
            transform.rotation = lastValidRotation;
            return;
        }

        Transform wp = trackWaypoints[lastPassedWaypointIndex];
        carRb.velocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;
        transform.position = wp.position + Vector3.up * 1f;
        transform.rotation = Quaternion.LookRotation(trackWaypoints[(lastPassedWaypointIndex + 1) % trackWaypoints.Length].position - wp.position);
    }


    void LateUpdate()
    {
        // Check if the car is airborne (not grounded)
        isAirborne = !IsGrounded();

        Move();
        Steer();
        Brake();
    }

    public void MoveInput(float input)
    {
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void GetInputs()
    {
        switch (control)
        {
            case ControlMode.Player1:
                moveInput = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
                steerInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
                isBraking = Input.GetKey(KeyCode.Q);
                break;

            case ControlMode.Player2:
                moveInput = Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
                steerInput = Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
                isBraking = Input.GetKey(KeyCode.Space);
                break;

            case ControlMode.Buttons:
                // In Buttons mode, inputs should be set externally via MoveInput() and SteerInput()
                // We'll assume brake is handled by a separate button that sets isBraking
                break;
        }
    }

    void Move()
    {
        if (isStunned)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0f;
            }
            return;
        }

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * maxAcceleration * speedBoostMultiplier;

        }
    }

    void Steer()
    {
        // Reduce steering when airborne
        float speedFactor = Mathf.Clamp01(carRb.velocity.magnitude / 20f);
        float adjustedSteer = steerInput * turnSensitivity * Mathf.Lerp(maxSteerAngle, maxSteerAngle * 0.5f, speedFactor);

        // If airborne, reduce the steering intensity
        if (isAirborne)
        {
            adjustedSteer *= 0.5f;  // Airborne steering sensitivity
        }

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, adjustedSteer, 0.1f);
            }
        }
    }

    void Brake()
    {
        float brakeForce = isBraking || moveInput == 0 || isStunned ? brakeAcceleration : 0;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.brakeTorque = brakeForce;
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
    public void ApplyTNTStun()
    {
        isStunned = true;
        stunTimer = stunDuration;

        carRb.velocity *= 0.3f;

        LoseCollectibles(); // 👈 Lose some on hit

        Debug.Log($"{gameObject.name} is stunned by TNT and slowed down!");
    }

    public int collectibleCount = 0;

    public void AddCollectible(int amount = 1)
    {
        collectibleCount += amount;
        Debug.Log($"{gameObject.name} collected! Total = {collectibleCount}");
    }

    public void LoseCollectibles(int amount = 3)
    {
        collectibleCount -= amount;
        if (collectibleCount < 0) collectibleCount = 0;
        Debug.Log($"{gameObject.name} lost {amount} collectibles! Remaining = {collectibleCount}");
    }

    public void ApplySpeedBoost(float boostMultiplier = 2f, float duration = 2f)
    {
        speedBoostMultiplier = boostMultiplier;
        boostTimer = duration;
        Debug.Log($"{gameObject.name} received a speed boost!");
    }



}
