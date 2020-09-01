using Debug;
using UnityEngine;

public class Player : Body
{
    private new Camera camera;

    // Movement
    private const float Speed = 12000f;
    private float forwardAcceleration;
    private float sidewaysAcceleration;
    private float yAcceleration;

    // Camera
    private float verticalBodyRotation;
    private const float MouseSensitivity = 150f;

    public new void Awake()
    {
        base.Awake();

        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        SaveUserMovementInput();
        ProcessCameraInput();
        ProcessDebugInput();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyGravity();
    }

    private void Move()
    {
        Transform cachedTransform = transform;

        Vector3 playerMotion = cachedTransform.forward * forwardAcceleration +
                               cachedTransform.right * sidewaysAcceleration +
                               cachedTransform.up * yAcceleration;
        playerMotion *= Time.deltaTime;
        
        rigidbody.AddForce(playerMotion);
    }

    private void SaveUserMovementInput()
    {
        // Move
        forwardAcceleration = CalculateMotion(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S));
        sidewaysAcceleration = CalculateMotion(Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.A));
        yAcceleration = CalculateMotion(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));
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
            Vector3 gravityForce = SolarSystem.ComputeGravitationalForce(this, celestialBody) / 50f; // Todo: do something with this number
            rigidbody.AddForce(gravityForce * Time.deltaTime);

            if (ShouldRotateTowardsCelestialBody(gravityForce, maxGravityForce, celestialBody))
            {
                maxGravityForce = gravityForce;
                maxGravityForceCelestialBody = celestialBody;
            }
        }

        if (maxGravityForceCelestialBody != null)
        {
            TopLeftCornerDebug.AddDebug($"Gravitated towards: {maxGravityForceCelestialBody.name}");
            TopLeftCornerDebug.AddDebug($"Gravity Magnitude: {maxGravityForce.magnitude}");
            TopLeftCornerDebug.AddDebug($"Distance to {maxGravityForceCelestialBody.name}: {(maxGravityForceCelestialBody.Position - Position).magnitude}");
            TopLeftCornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity(maxGravityForceCelestialBody)}");
            RotateTowardsCelestialBody(maxGravityForceCelestialBody);
        }
    }

    private bool ShouldRotateTowardsCelestialBody(Vector3 gravityForce, Vector3 maxGravityForce, CelestialBody celestialBody)
    {
        // We only rotate to a body with the most gravity force
        if (gravityForce.magnitude < maxGravityForce.magnitude)
        {
            return false;
        }

        // We only rotate to a body if it is nearby
        if ((celestialBody.Position - Position).magnitude > 600f)
        {
            return false;
        }

        // We don't rotate to the sun, because it's impossible to land on it
        if (celestialBody.name == "Sun")
        {
            return false;
        }

        TopLeftCornerDebug.AddDebug($"Rotating towards {celestialBody.name}");
        return true;
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