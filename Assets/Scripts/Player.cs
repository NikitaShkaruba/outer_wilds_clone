using Debug;
using UnityEngine;

public class Player : Body
{
    private new Camera camera;
    
    private const float Speed = 12000f;
    
    // Camera
    private float verticalBodyRotation;
    private const float MouseSensitivity = 150f;
    
    // Max gravitation to something to rotate to it
    private Vector3 maxGravityForce;
    public CelestialBody maxCelestialBody;

    public new void Awake()
    {
        base.Awake();
        camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        ProcessMovementKeys();
        ProcessCameraKeys();
        ProcessDebugKeys();
    }

    private void ProcessMovementKeys()
    {
        bool wKeyPressed = Input.GetKey(KeyCode.W);
        bool aKeyPressed = Input.GetKey(KeyCode.A);
        bool sKeyPressed = Input.GetKey(KeyCode.S);
        bool dKeyPressed = Input.GetKey(KeyCode.D);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl);

        float forwardMotion = CalculateMotion(wKeyPressed, sKeyPressed);
        float sidewaysMotion = CalculateMotion(dKeyPressed, aKeyPressed);
        float yMotion = CalculateMotion(isShiftPressed, isCtrlPressed);

        Transform cachedTransform = transform;
        Vector3 playerMotion = cachedTransform.forward * forwardMotion +
                               cachedTransform.right * sidewaysMotion +
                               cachedTransform.up * yMotion;

        playerMotion *= Time.deltaTime;

        rigidbody.AddForce(playerMotion);

    }

    private void ProcessCameraKeys()
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

    private static void ProcessDebugKeys()
    {
        bool changeCursorLockStateKeyDown = Input.GetKeyDown(KeyCode.F1);
        bool toggleDebugKeyDown = Input.GetKeyDown(KeyCode.F2);

        if (changeCursorLockStateKeyDown)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (toggleDebugKeyDown)
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

    public void RememberRotation(Vector3 force, CelestialBody celestialBody)
    {
        if (force.magnitude > maxGravityForce.magnitude)
        {
            maxGravityForce = force;
            maxCelestialBody = celestialBody;
        }
    }

    public void ResetRotation()
    {
        maxGravityForce = Vector3.zero;
        maxCelestialBody = null;
    }

    public void RotateTowardsGravity()
    {
        Transform cachedTransform = transform;
        Quaternion cachedTransformRotation = cachedTransform.rotation;
        
        Vector3 gravityForceDirection = (cachedTransform.position - maxCelestialBody.Position).normalized;
        Vector3 playerUp = cachedTransform.up;
        Quaternion neededRotation = Quaternion.FromToRotation(playerUp, gravityForceDirection) * cachedTransformRotation;
        
        cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, neededRotation, Time.deltaTime);
        cachedTransform.rotation = cachedTransformRotation;
    }
}