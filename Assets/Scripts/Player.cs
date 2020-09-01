﻿using Debug;
using UnityEngine;

public class Player : Body
{
    private new Camera camera;

    private const float Speed = 12000f;

    // Camera
    private float verticalBodyRotation;
    private const float MouseSensitivity = 150f;

    public new void Awake()
    {
        base.Awake();

        camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        ProcessUserInput();
        ApplyGravity();
    }

    private void ProcessUserInput()
    {
        ProcessMovementInput();
        ProcessCameraInput();
        ProcessDebugInput();
    }

    private void ProcessMovementInput()
    {
        Transform cachedTransform = transform;

        float forwardMotion = CalculateMotion(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S));
        float sidewaysMotion = CalculateMotion(Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.A));
        float yMotion = CalculateMotion(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));

        Vector3 playerMotion = (cachedTransform.forward * forwardMotion + cachedTransform.right * sidewaysMotion + cachedTransform.up * yMotion) * Time.deltaTime;
        rigidbody.AddForce(playerMotion);
    }

    private void ProcessCameraInput()
    {
        float horizontalMouseOffset = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float verticalMouseOffset = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        bool rotateButtonPressed = Input.GetKey(KeyCode.R);

        verticalBodyRotation -= verticalMouseOffset;
        verticalBodyRotation = Mathf.Clamp(verticalBodyRotation, -90f, 90f); // We don't want our player to roll over with the camera :)
        camera.transform.localRotation = Quaternion.Euler(verticalBodyRotation, 0f, 0f);

        if (rotateButtonPressed)
        {
            transform.Rotate(Vector3.forward * -horizontalMouseOffset);
        }
        else
        {
            transform.Rotate(Vector3.up * horizontalMouseOffset);
        }
    }

    private static void ProcessDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TopLeftCornerDebug topLeftCornerDebug2 = FindObjectOfType<TopLeftCornerDebug>();
            topLeftCornerDebug2.isHidden = !topLeftCornerDebug2.isHidden;
        }
    }

    private static float CalculateMotion(bool oneDirectionKeyPressed, bool otherDirectionKeyPressed)
    {
        if (oneDirectionKeyPressed)
        {
            return Speed;
        }
        else if (otherDirectionKeyPressed)
        {
            return -Speed;
        }

        return 0f;
    }

    private void ApplyGravity()
    {
        Vector3 maxGravityForce = Vector3.zero;
        CelestialBody maxGravityForceCelestialBody = null;

        foreach (CelestialBody celestialBody in SolarSystem.CelestialBodies)
        {
            Vector3 gravityForce = SolarSystem.ComputeGravitationalForce(this, celestialBody) / 1200f; // 400f just for now. I don't understand why it works
            rigidbody.AddForce(gravityForce);

            if (gravityForce.magnitude > maxGravityForce.magnitude)
            {
                maxGravityForce = gravityForce;
                maxGravityForceCelestialBody = celestialBody;
            }
        }

        if (maxGravityForceCelestialBody != null)
        {
            TopLeftCornerDebug.AddDebug($"Gravitated towards: {maxGravityForceCelestialBody.name}");
            TopLeftCornerDebug.AddDebug($"Gravity Magnitude: {maxGravityForce.magnitude}");
            TopLeftCornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity(maxGravityForceCelestialBody)}");
            RotateTowardsCelestialBody(maxGravityForceCelestialBody);
        }
    }

    private void RotateTowardsCelestialBody(CelestialBody celestialBody)
    {
        Transform cachedTransform = transform;
        Quaternion cachedTransformRotation = cachedTransform.rotation;

        Vector3 gravityForceDirection = (cachedTransform.position - celestialBody.Position).normalized;
        Vector3 playerUp = cachedTransform.up;
        Quaternion neededRotation = Quaternion.FromToRotation(playerUp, gravityForceDirection) * cachedTransformRotation;

        cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, neededRotation, Time.deltaTime);
        cachedTransform.rotation = cachedTransformRotation;
    }

    private string FormatPlayerVelocity(CelestialBody celestialBody)
    {
        if (celestialBody == null)
        {
            return "";
        }

        Vector3 playerVelocityCached = rigidbody.velocity;
        Vector3 maxCelestialBodyVelocity = celestialBody.rigidbody.velocity;

        float playerVelocityX = playerVelocityCached.x - maxCelestialBodyVelocity.x;
        float playerVelocityY = playerVelocityCached.y - maxCelestialBodyVelocity.y;
        float playerVelocityZ = playerVelocityCached.z - maxCelestialBodyVelocity.z;

        const string stringFormat = "####0";
        string playerVelocityXText = playerVelocityX.ToString(stringFormat);
        string playerVelocityYText = playerVelocityY.ToString(stringFormat);
        string playerVelocityZText = playerVelocityZ.ToString(stringFormat);

        return $"({playerVelocityXText}, {playerVelocityYText}, {playerVelocityZText})";
    }
}