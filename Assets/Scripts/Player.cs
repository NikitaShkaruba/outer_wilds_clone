using Debug;
using Tools.SpaceShipParts;
using UnityEngine;

public class Player : Body
{
    public new Camera camera;

    // User input
    private Vector3 wantedMovement;
    private bool wantsToJump;

    // Movement
    private const float ThrustersAcceleration = 4000f;
    private const float LegsAcceleration = 3000f;
    private const float MaxLegsSpeed = 12f;
    private const float JumpPower = 1200f;

    // Ground check
    [SerializeField] private LayerMask groundMask;
    private const float GroundDistance = 0.4f;
    private CelestialBody maxGravityForceCelestialBody;

    // Camera
    private float verticalBodyRotation;
    private const float MouseSensitivity = 150f;

    // SpaceShip interaction
    public bool isBuckledUp;
    public bool buckleUpTransitionGoing;
    private SpaceShipSeat spaceShipSeatToBuckleUp;

    public new void Awake()
    {
        base.Awake();

        camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        SaveUserMovementInput();
        ProcessCameraInput();
        ProcessDebugInput();
    }

    private void FixedUpdate()
    {
        if (buckleUpTransitionGoing)
        {
            DoBucklingUpPiece();
            return;
        }

        if (isBuckledUp)
        {
            return;
        }

        Move();
        ApplyGravity();

        TopLeftCornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity()}");
    }

    private void Move()
    {
        Vector3 playerMotion = GetPlayerMotion();
        rigidbody.AddForce(playerMotion);

        if (wantsToJump && IsOnTheGround())
        {
            Vector3 jumpMotion = transform.up * JumpPower;
            rigidbody.AddForce(jumpMotion);
        }
    }

    private Vector3 GetPlayerMotion()
    {
        Transform cachedTransform = transform;

        Vector3 playerHorizontalMotion = cachedTransform.forward * wantedMovement.x +
                                         cachedTransform.right * wantedMovement.z;
        Vector3 playerVerticalMotion = cachedTransform.up * wantedMovement.y;

        if (IsOnTheGround())
        {
            Vector3 relativeVelocity = GetRelativeVelocity();
            if (relativeVelocity.magnitude < MaxLegsSpeed)
            {
                // Player uses legs when on the ground
                playerHorizontalMotion *= LegsAcceleration;
            }
            else
            {
                // Player can't accelerate as much as he wants :)
                playerHorizontalMotion = Vector3.zero;
            }
        }
        else
        {
            // Player uses thrusters when in space
            playerHorizontalMotion *= ThrustersAcceleration;
        }

        // Vertical motion always uses thrusters
        playerVerticalMotion *= ThrustersAcceleration;

        Vector3 playerMotion = playerHorizontalMotion + playerVerticalMotion;
        playerMotion *= Time.deltaTime;

        return playerMotion;
    }

    private bool IsOnTheGround()
    {
        Transform cachedTransform = transform;
        Vector3 groundCoordinate = cachedTransform.position - cachedTransform.up;

        return Physics.CheckSphere(groundCoordinate, GroundDistance, groundMask);
    }

    private void SaveUserMovementInput()
    {
        // Move
        wantedMovement.x = CalculateDirection(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S));
        wantedMovement.z = CalculateDirection(Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.A));
        wantedMovement.y = CalculateDirection(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));

        // Jump
        wantsToJump = Input.GetKey(KeyCode.Space);
    }

    private void ProcessCameraInput()
    {
        if (buckleUpTransitionGoing || isBuckledUp)
        {
            return;
        }

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

    private static float CalculateDirection(bool oneDirectionKeyPressed, bool otherDirectionKeyPressed)
    {
        if (oneDirectionKeyPressed)
        {
            return 1f;
        }
        else if (otherDirectionKeyPressed)
        {
            return -1f;
        }

        return 0f;
    }

    private void ApplyGravity()
    {
        Vector3 maxGravityForce = Vector3.zero;
        maxGravityForceCelestialBody = null;

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

    private string FormatPlayerVelocity()
    {
        Vector3 velocity = maxGravityForceCelestialBody == null ? rigidbody.velocity : GetRelativeVelocity();

        const string stringFormat = "####0";
        string playerVelocityXText = velocity.x.ToString(stringFormat);
        string playerVelocityYText = velocity.y.ToString(stringFormat);
        string playerVelocityZText = velocity.z.ToString(stringFormat);

        return $"({playerVelocityXText}, {playerVelocityYText}, {playerVelocityZText})";
    }

    private Vector3 GetRelativeVelocity()
    {
        if (maxGravityForceCelestialBody == null)
        {
            return Vector3.zero;
        }

        return rigidbody.velocity - maxGravityForceCelestialBody.rigidbody.velocity;
    }

    public void StartBucklingUp(SpaceShipSeat spaceShipSeat)
    {
        // Change state
        buckleUpTransitionGoing = true;
        spaceShipSeatToBuckleUp = spaceShipSeat;
        transform.SetParent(spaceShipSeat.transform);
        
        // Disable movement
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
    }

    public void Unbuckle()
    {
        // Enable movement again
        rigidbody.velocity = spaceShipSeatToBuckleUp.spaceShip.rigidbody.velocity;
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;
        
        // Change state
        transform.SetParent(null);
        isBuckledUp = false;
        buckleUpTransitionGoing = false; // In case we stopped buckling up during the transition
        spaceShipSeatToBuckleUp = null;
    }

    private void DoBucklingUpPiece()
    {
        // Move player into the chair
        Vector3 desiredPosition = new Vector3(0, 0.5f, 1.1f); // A little bit forward and up of the (0,0,0) coordinates of the chair
        Vector3 positionDifference = desiredPosition - transform.localPosition;
        Vector3 positionAddition = positionDifference; // difference - is what we should add in order to become the same
        positionAddition *= 2f; // Speedup the process a bit
        positionAddition *= Time.deltaTime; // Include physics rendering step
        transform.localPosition += positionAddition;

        // Rotate player body and camera to (0,0,0)
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Unity.BugFixes.TransformBlenderEulerAngles(new Vector3(0, 0, 0))), Time.deltaTime);
        camera.transform.localRotation = Quaternion.Slerp(camera.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime);

        const float positionCheckThreshold = 0.01f;
        bool gotToTheChair = Mathf.Abs(positionDifference.x) <= positionCheckThreshold &&
                             Mathf.Abs(positionDifference.y) <= positionCheckThreshold &&
                             Mathf.Abs(positionDifference.z) <= positionCheckThreshold;

        // Yes, I don't account rotation on purpose
        if (gotToTheChair)
        {
            buckleUpTransitionGoing = false;
            isBuckledUp = true;
            UnityEngine.Debug.Log("Got into chair");
        }
    }
}