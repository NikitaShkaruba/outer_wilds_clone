using Common;
using PlayerLogic;
using PlayerTools;
using PlayerTools.SpaceShipParts;
using PlayerTools.SpaceSuit;
using UI.Debug;
using UnityEngine;
using UnityEngine.UI;
using Universe;

[RequireComponent(typeof(PlayerInput))]
public class Player : SpaceBody
{
    public new Camera camera;
    private new Transform transform;

    // Movement
    [SerializeField] private float thrustersPower;
    [SerializeField] private float moveSpeed;

    // Jump
    private float acceleratedJumpPower;
    [SerializeField] private float jumpPowerAccelerationPerTick;
    [SerializeField] private float maxJumpPower;
    [SerializeField] private float playerShrinkOnJumpSpeed;
    private Vector3? scaleBeforeJumpShrinking;

    // Ground check
    private LayerMask groundCheckLayerMask;

    // Camera
    private float verticalBodyRotation;

    // SpaceShip interaction
    [HideInInspector] public bool isBuckledUp;
    [HideInInspector] public bool buckleUpTransitionGoing;
    [HideInInspector] public SpaceShip pilotedSpaceShip;

    // Death screen
    [SerializeField] private Image deathBlackFadeImage;

    // You are taking damage text
    [SerializeField] private GameObject youAreTakingDamageText;
    [SerializeField] private float hideYouAreTakingDamageTextTimer;
    [SerializeField] private float hideYouAreTakingDamageTextTimerTime;

    // ----- Refactoring ----

    [Header("Health")]
    [SerializeField] private float healthRefillSpeed;
    [SerializeField] private SpaceSuitHealthIndicator healthIndicator;
    private Damageable damageable;
    public bool HasFullHealthPoints => damageable.HasFullHealthPoints;

    [Header("Oxygen")]
    [SerializeField] private float oxygenDepletionSpeed;
    [SerializeField] private float oxygenRefillSpeed;
    [SerializeField] private SpaceSuitBar oxygenBar;
    private Tank oxygenTank;

    [Header("Jetpack fuel")]
    [SerializeField] private float fuelDepletionSpeed;
    [SerializeField] private float fuelRefillSpeed;
    [SerializeField] private SpaceSuitBar fuelBar;
    private Tank fuelTank;
    public bool IsFuelTankFull => fuelTank.IsFull;

    [Header("Jetpack super-fuel")]
    [SerializeField] private float superFuelDepletionSpeed;
    [SerializeField] private float superFuelRestorationSpeed;
    [SerializeField] private float superFuelPowerMultiplier;
    [SerializeField] private SpaceSuitBar superFuelBar;
    private Tank superFuelTank;

    // Ungrouped
    private PlayerInput playerInput;
    private bool IsDead => damageable.HasNoHealthPoints || oxygenTank.IsEmpty;
    private bool healthAndFuelRefilling;

    public new void Awake()
    {
        base.Awake();

        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();

        groundCheckLayerMask = LayerMask.GetMask("Planets", "Objects");

        Cursor.lockState = CursorLockMode.Locked;

        damageable = new Damageable(100f);
        oxygenTank = new Tank();
        fuelTank = new Tank();
        superFuelTank = new Tank();
    }

    private void Update()
    {
        UpdateSpaceSuitIndicators();
        UpdateYouAreTakingDamageText();

        if (healthAndFuelRefilling)
        {
            RefillHealthAndFuel();
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        if (IsDead)
        {
            FadeScreen();
            return;
        }

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
        BreatheOxygen();

        CornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity()}");
        CornerDebug.AddDebug("IsOnTheGround = " + IsGrounded());
    }

    private void Move()
    {
        Transform cachedTransform = transform;

        Vector3 playerHorizontalMotion = cachedTransform.forward * playerInput.movement.x +
                                         cachedTransform.right * playerInput.movement.z;
        Vector3 playerVerticalMotion = cachedTransform.up * playerInput.movement.y;

        if (IsGrounded())
        {
            Vector3 playerPositionAddition = playerHorizontalMotion;
            playerPositionAddition *= moveSpeed;
            playerPositionAddition *= Time.deltaTime;

            // Movement by foot with AddForce is buggy, so for now this will work.
            rigidbody.MovePosition(rigidbody.position + playerPositionAddition);

            ProcessJumpLogic();
        }
        else if (HasPropellant())
        {
            Vector3 horizontalThrustersForce = playerHorizontalMotion;
            horizontalThrustersForce *= thrustersPower;
            horizontalThrustersForce *= Time.deltaTime;

            rigidbody.AddForce(horizontalThrustersForce);
            DepletePropellant();
        }

        float superFuelMultiplier = 1f;
        if (playerInput.movement.y > 0f && playerInput.jump)
        {
            if (!superFuelTank.IsEmpty)
            {
                superFuelMultiplier = superFuelPowerMultiplier;
                superFuelTank.Deplete(superFuelDepletionSpeed);
            }
        }
        else
        {
            if (!superFuelTank.IsFull && HasPropellant())
            {
                // Fill superFuelTank with propellant 
                DepletePropellant();
                superFuelTank.Fill(superFuelRestorationSpeed);
            }
        }

        if (!Mathf.Approximately(playerInput.movement.y, 0f) && HasPropellant())
        {
            Vector3 verticalThrustersForce = playerVerticalMotion;
            verticalThrustersForce *= thrustersPower;
            verticalThrustersForce *= superFuelMultiplier;
            verticalThrustersForce *= Time.deltaTime;

            rigidbody.AddForce(verticalThrustersForce);
            DepletePropellant();
        }
    }

    private void ProcessJumpLogic()
    {
        if (playerInput.jump && acceleratedJumpPower <= maxJumpPower)
        {
            // Remember original scale
            if (scaleBeforeJumpShrinking == null)
            {
                scaleBeforeJumpShrinking = transform.localScale;
            }

            // Shrink player a bit
            transform.localScale -= new Vector3(0, playerShrinkOnJumpSpeed, 0);

            // Accelerate jump power
            acceleratedJumpPower += jumpPowerAccelerationPerTick;
        }
        else if (!playerInput.jump && acceleratedJumpPower > 0f)
        {
            // Resize player back
            System.Diagnostics.Debug.Assert(scaleBeforeJumpShrinking != null, nameof(scaleBeforeJumpShrinking) + " != null");
            transform.localScale = (Vector3) scaleBeforeJumpShrinking;

            // Add jump force
            Vector3 jumpMotion = transform.up * acceleratedJumpPower;
            rigidbody.AddForce(jumpMotion);

            acceleratedJumpPower = 0f;
            scaleBeforeJumpShrinking = null;
        }
    }

    private void UpdateSpaceSuitIndicators()
    {
        oxygenBar.UpdatePercentage(oxygenTank.FilledPercentage);
        fuelBar.UpdatePercentage(fuelTank.FilledPercentage);
        superFuelBar.UpdatePercentage(superFuelTank.FilledPercentage);
        healthIndicator.UpdatePercentage(damageable.HealthPoints);
    }

    private void UpdateYouAreTakingDamageText()
    {
        if (hideYouAreTakingDamageTextTimer < 0 && !youAreTakingDamageText.activeSelf)
        {
            return;
        }

        hideYouAreTakingDamageTextTimer -= Time.fixedDeltaTime;

        if (hideYouAreTakingDamageTextTimer < 0)
        {
            youAreTakingDamageText.SetActive(false);
        }
    }

    private void PilotSpaceShip()
    {
        pilotedSpaceShip.Pilot(playerInput.movement, playerInput.rotation, playerInput.alternativeRotate);
    }

    private void BreatheOxygen()
    {
        if (!oxygenTank.IsEmpty)
        {
            oxygenTank.Deplete(oxygenDepletionSpeed);
        }
    }

    public void FillOxygenTanks()
    {
        if (!oxygenTank.IsFull)
        {
            oxygenTank.Fill(oxygenRefillSpeed);
        }
    }

    private void DepletePropellant(float multiplier = 1f)
    {
        float depletionSpeed = fuelDepletionSpeed * multiplier;

        if (!fuelTank.IsEmpty)
        {
            fuelTank.Deplete(depletionSpeed);
        }
        else if (!oxygenTank.IsEmpty)
        {
            oxygenTank.Deplete(depletionSpeed);
        }
    }

    private bool HasPropellant()
    {
        return !fuelTank.IsEmpty || !oxygenTank.IsEmpty;
    }

    public void Hurt(float healthPercentageToRemove)
    {
        damageable.Damage(healthPercentageToRemove);

        youAreTakingDamageText.SetActive(true);
        hideYouAreTakingDamageTextTimer = hideYouAreTakingDamageTextTimerTime;
    }

    private void FadeScreen()
    {
        Color nextDeathBlackFadeImage = deathBlackFadeImage.color;
        nextDeathBlackFadeImage.a += 0.01f;

        deathBlackFadeImage.color = nextDeathBlackFadeImage;
    }

    private bool IsGrounded()
    {
        const float distanceFromBodyCenterToGround = 1.1f;

        return Physics.Raycast(transform.position, -transform.up, distanceFromBodyCenterToGround, groundCheckLayerMask);
    }

    public void StartRefillingStocksFromShip()
    {
        healthAndFuelRefilling = true;
    }

    private void RefillHealthAndFuel()
    {
        damageable.Heal(healthRefillSpeed);
        fuelTank.Fill(fuelRefillSpeed);

        if (damageable.HasFullHealthPoints && fuelTank.IsFull)
        {
            healthAndFuelRefilling = false;
        }
    }

    private void Rotate()
    {
        if (buckleUpTransitionGoing || isBuckledUp)
        {
            return;
        }

        float horizontalMouseOffset = playerInput.rotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
        float verticalMouseOffset = playerInput.rotation.y * GameSettings.MouseSensitivity * Time.deltaTime;

        verticalBodyRotation -= verticalMouseOffset;
        verticalBodyRotation = Mathf.Clamp(verticalBodyRotation, -90f, 90f); // We don't want our player to roll over with the camera :)
        camera.transform.localRotation = Quaternion.Euler(verticalBodyRotation, 0f, 0f);

        if (playerInput.alternativeRotate)
        {
            transform.Rotate(Vector3.forward * -horizontalMouseOffset);
        }
        else
        {
            transform.Rotate(Vector3.up * horizontalMouseOffset);
        }
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
        pilotedSpaceShip = spaceShipSeat.spaceShip;
        transform.SetParent(spaceShipSeat.transform);

        // Disable movement
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
    }

    public void Unbuckle()
    {
        // Enable movement again
        rigidbody.velocity = pilotedSpaceShip.rigidbody.velocity;
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;

        // Change state
        transform.SetParent(null);
        isBuckledUp = false;
        buckleUpTransitionGoing = false; // In case we stopped buckling up during the transition
        pilotedSpaceShip = null;
    }

    private void DoBucklingUpPiece()
    {
        Vector3 transformCachedPosition = transform.localPosition;
        Quaternion cachedTransformRotation = transform.localRotation;
        Quaternion cachedCameraTransformRotation = camera.transform.localRotation;

        // Move player into the chair
        Vector3 desiredPosition = new Vector3(0, 0.5f, 1.1f); // A little bit forward and up of the (0,0,0) coordinates of the chair
        Vector3 positionDifference = desiredPosition - transformCachedPosition;
        Vector3 positionAddition = positionDifference; // difference - is what we should add in order to become the same
        positionAddition *= 2f; // Speedup the process a bit
        positionAddition *= Time.deltaTime; // Include physics rendering step
        transformCachedPosition += positionAddition;
        transform.localPosition = transformCachedPosition;

        // Rotate player body to (0,0,0)
        Quaternion wantedTransformRotation = Quaternion.Euler(BlenderBugFixes.TransformBlenderEulerAngles(new Vector3(0, 0, 0)));
        cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, wantedTransformRotation, Time.deltaTime);
        transform.localRotation = cachedTransformRotation;

        // Rotate player camera to (0,0,0)
        Quaternion wantedCameraRotation = Quaternion.Euler(0, 0, 0);
        cachedCameraTransformRotation = Quaternion.Slerp(cachedCameraTransformRotation, wantedCameraRotation, Time.deltaTime);
        camera.transform.localRotation = cachedCameraTransformRotation;

        // Check that everything is positioned properly
        const float positionCheckThreshold = 0.01f;
        bool movedToTheChair = Mathf.Abs(positionDifference.x) <= positionCheckThreshold &&
                               Mathf.Abs(positionDifference.y) <= positionCheckThreshold &&
                               Mathf.Abs(positionDifference.z) <= positionCheckThreshold;

        // Check that everything is rotated properly
        const float rotationCheckThreshold = 0.00001f;
        bool transformRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(cachedTransformRotation, wantedTransformRotation)) < rotationCheckThreshold;
        bool cameraRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(cachedCameraTransformRotation, wantedCameraRotation)) < rotationCheckThreshold;

        if (movedToTheChair && transformRotatedProperly && cameraRotatedProperly)
        {
            buckleUpTransitionGoing = false;
            isBuckledUp = true;
        }
    }
}
