﻿using System;
using Common;
using PlayerLogic;
using PlayerTools;
using PlayerTools.SpaceShipParts;
using UI.Debug;
using UnityEngine;
using Universe;

[RequireComponent(typeof(PlayerInput))]
public class Player : SpaceBody
{
    public new Camera camera;
    private new Transform transform;

    // Movement
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

    // You are taking damage text
    [SerializeField] private GameObject youAreTakingDamageText;
    [SerializeField] private float hideYouAreTakingDamageTextTimer;
    [SerializeField] private float hideYouAreTakingDamageTextTimerTime;

    // UI which needs refactoring

    private bool healthAndFuelRefilling;

    [Header("Health")]
    [SerializeField] private float healthRefillSpeed;

    // ----- Refactored ----

    private PlayerInput playerInput;
    public Damageable Damageable;
    public SpaceSuit SpaceSuit;

    private bool hasSomethingToBreathe = true;
    private bool isDead;

    public bool WantsToRefillFuelTank => !SpaceSuit.IsFuelTankFull;
    public bool WantsToHealUp => Damageable.HasFullHealthPoints;

    // Ui Events
    public event Action OnDeath;

    public new void Awake()
    {
        base.Awake();

        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();

        groundCheckLayerMask = LayerMask.GetMask("Planets", "Objects");

        Damageable = new Damageable(100f);
        SpaceSuit = new SpaceSuit();

        Damageable.OnDeath += Die;
    }

    public void OnDestroy()
    {
        Damageable.OnDeath -= Die;
    }

    private void Update()
    {
        UpdateYouAreTakingDamageText();

        if (healthAndFuelRefilling)
        {
            RefillHealthAndFuel();
        }
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        SpaceSuit.Tick();

        if (isDead)
        {
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
            rigidbody.MovePosition(rigidbody.position + playerPositionAddition); // Movement by foot with AddForce is buggy, so for now this will work.

            ProcessJumpLogic();
        }
        else
        {
            Vector3 horizontalThrustersForce = playerHorizontalMotion;
            horizontalThrustersForce *= SpaceSuit.FireHorizontalThrusters();
            horizontalThrustersForce *= Time.deltaTime;
            rigidbody.AddForce(horizontalThrustersForce);
        }

        if (!Mathf.Approximately(playerInput.movement.y, 0f))
        {
            bool useSuperFuel = playerInput.movement.y > 0f && playerInput.jump;

            Vector3 verticalThrustersForce = playerVerticalMotion;
            verticalThrustersForce *= SpaceSuit.FireVerticalThrusters(useSuperFuel);
            verticalThrustersForce *= Time.deltaTime;

            rigidbody.AddForce(verticalThrustersForce);
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
        hasSomethingToBreathe = SpaceSuit.GiveOxygenToBreathe();

        if (!hasSomethingToBreathe)
        {
            // Todo: add little delay (player can survive without oxygen for 30 seconds or so)
            Die();
        }
    }

    public void FillOxygenTanks()
    {
        SpaceSuit.FillOxygenTank();
    }

    public void Hurt(float healthPercentageToRemove)
    {
        Damageable.Damage(healthPercentageToRemove);

        youAreTakingDamageText.SetActive(true);
        hideYouAreTakingDamageTextTimer = hideYouAreTakingDamageTextTimerTime;
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
        Damageable.Heal(healthRefillSpeed);
        SpaceSuit.FillFuelTank();

        if (Damageable.HasFullHealthPoints && SpaceSuit.IsFuelTankFull)
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
