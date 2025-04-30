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
        //public GameObject wheelEffectObj;
        //public ParticleSystem smokeParticle;
        public Axel axel;
    }

    [Header("Control Settings")]
    public PlayerControl control = PlayerControl.Player1;

    [Header("Car Settings")]
    public float maxAcceleration = 20f;
    public float brakeAcceleration = 2500f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 25.0f;
    public Vector3 _centerOfMass;
    public List<Wheel> wheels;

    float moveInput;
    float steerInput;
    bool isBraking;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

        // Add realistic resistance
        carRb.drag = 0.2f;
        carRb.angularDrag = 2f;

        // Improve tire grip
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
        WheelEffects();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    void GetInputs()
    {
        if (control == PlayerControl.Player1)
        {
            moveInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
            steerInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            isBraking = Input.GetKey(KeyCode.Q);
        }
        else if (control == PlayerControl.Player2)
        {
            moveInput = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
            steerInput = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
            isBraking = Input.GetKey(KeyCode.Space);
        }
    }

    void Move()
    {
        float torque = moveInput * maxAcceleration * 150f;
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = torque;
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

    void Brake()
    {
        float brakeForce = isBraking ? brakeAcceleration : 0f;
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

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (isBraking && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded && carRb.velocity.magnitude >= 10.0f)
            {
                //wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                //wheel.smokeParticle.Emit(1);
            }
        }
    }
}
