using Debug;
using Tools.SpaceShipParts;
using UnityEngine;

public class Player : SpaceBody
{
    public new Camera camera;
    private new Transform transform;

    // User input
    private Vector3 wantedMovement;
    private Vector2 wantedRotation;
    private bool wantsToJump;
    private bool wantsToRotateAroundForwardVector;

    // Movement
    private const float ThrustersAcceleration = 4000f;
    private const float LegsAcceleration = 3000f;
    private const float MaxLegsSpeed = 12f;
    private const float JumpPower = 1200f;

    // Ground check
    [SerializeField] private LayerMask groundMask;
    private const float GroundDistance = 0.4f;

    // Camera
    private float verticalBodyRotation;

    // SpaceShip interaction
    public bool isBuckledUp;
    public bool buckleUpTransitionGoing;
    private SpaceShipSeat spaceShipSeatToBuckleUp;

    public new void Awake()
    {
        base.Awake();

        camera = GetComponentInChildren<Camera>();
        transform = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        SaveUserMovementInput();
        SaveUserRotationInput();

        ProcessDebugInput();
    }

    private void FixedUpdate()
    {
        if (buckleUpTransitionGoing)
        {
            DoBucklingUpPiece();
            PilotSpaceShip();
            return;
        }

        if (isBuckledUp)
        {
            PilotSpaceShip();
            return;
        }

        Move();
        Rotate();
        ApplyGravity();

        TopLeftCornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity()}");
    }

    private void Move()
    {
        Vector3 playerMotion = GetBodyMotion();
        rigidbody.AddForce(playerMotion);

        if (wantsToJump && IsOnTheGround())
        {
            Vector3 jumpMotion = transform.up * JumpPower;
            rigidbody.AddForce(jumpMotion);
        }
    }

    private void PilotSpaceShip()
    {
        spaceShipSeatToBuckleUp.spaceShip.Pilot(wantedMovement, wantedRotation, wantsToRotateAroundForwardVector);
    }

    private Vector3 GetBodyMotion()
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

    private void SaveUserRotationInput()
    {
        wantedRotation.x = Input.GetAxis("Mouse X");
        wantedRotation.y = Input.GetAxis("Mouse Y");

        wantsToRotateAroundForwardVector = Input.GetKey(KeyCode.R);
    }

    private void Rotate()
    {
        if (buckleUpTransitionGoing || isBuckledUp)
        {
            return;
        }

        float horizontalMouseOffset = wantedRotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
        float verticalMouseOffset = wantedRotation.y * GameSettings.MouseSensitivity * Time.deltaTime;

        verticalBodyRotation -= verticalMouseOffset;
        verticalBodyRotation = Mathf.Clamp(verticalBodyRotation, -90f, 90f); // We don't want our player to roll over with the camera :)
        camera.transform.localRotation = Quaternion.Euler(verticalBodyRotation, 0f, 0f);

        if (wantsToRotateAroundForwardVector)
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

    private string FormatPlayerVelocity()
    {
        Vector3 velocity = BodyToGravitateTowards == null ? rigidbody.velocity : GetRelativeVelocity();

        const string stringFormat = "####0";
        string playerVelocityXText = velocity.x.ToString(stringFormat);
        string playerVelocityYText = velocity.y.ToString(stringFormat);
        string playerVelocityZText = velocity.z.ToString(stringFormat);

        return $"({playerVelocityXText}, {playerVelocityYText}, {playerVelocityZText})";
    }

    private Vector3 GetRelativeVelocity()
    {
        if (BodyToGravitateTowards == null)
        {
            return Vector3.zero;
        }

        return rigidbody.velocity - BodyToGravitateTowards.rigidbody.velocity;
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
        Vector3 transformCachedPosition = transform.localPosition;

        // Move player into the chair
        Vector3 desiredPosition = new Vector3(0, 0.5f, 1.1f); // A little bit forward and up of the (0,0,0) coordinates of the chair
        Vector3 positionDifference = desiredPosition - transformCachedPosition;
        Vector3 positionAddition = positionDifference; // difference - is what we should add in order to become the same
        positionAddition *= 2f; // Speedup the process a bit
        positionAddition *= Time.deltaTime; // Include physics rendering step
        transformCachedPosition += positionAddition;
        transform.localPosition = transformCachedPosition;

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
