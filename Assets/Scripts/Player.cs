using System;
using Common;
using PlayerLogic;
using PlayerTools;
using PlayerTools.SpaceShipParts;
using UI.Debug;
using UnityEngine;
using Universe;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpaceSuit))]
public class Player : SpaceBody
{
    // Internal unity components
    public new Camera camera;
    private new Transform transform;

    // Internal components
    public Damageable Damageable;
    public SpaceSuit spaceSuit;
    private PlayerInput playerInput;
    private Leggable leggable;
    private Jumpable jumpable;

    // External components
    public SpaceShipSeat buckledUpSpaceShipSeat;

    // Some fields
    private float headVerticalRotation;
    private bool hasSomethingToBreathe = true;
    private bool isDead;

    // Ui Events
    public event Action OnDeath;

    public new void Awake()
    {
        base.Awake();

        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        spaceSuit = GetComponent<SpaceSuit>();

        Damageable = new Damageable(100f);
        leggable = new Leggable(this);
        jumpable = new Jumpable(this);

        Damageable.OnDeath += Die;
    }

    public void OnDestroy()
    {
        Damageable.OnDeath -= Die;
    }

    private void FixedUpdate()
    {
        BreatheOxygen();

        if (IsBuckledUp())
        {
            PilotSpaceShip();
            return;
        }

        ApplyGravity(); // We don't need gravity when we're sitting in a chair

        if (isDead)
        {
            return;
        }

        Move();
        Rotate();

        CornerDebug.AddDebug($"Player velocity: {FormatPlayerVelocity()}");
        CornerDebug.AddDebug("IsOnTheGround = " + leggable.IsGrounded());
    }

    private void Move()
    {
        Transform cachedTransform = transform;

        Vector3 playerHorizontalMotion = cachedTransform.forward * playerInput.movement.x +
                                         cachedTransform.right * playerInput.movement.z;
        Vector3 playerVerticalMotion = cachedTransform.up * playerInput.movement.y;

        // Walk by foot or fire vertical thrusters
        if (leggable.IsGrounded())
        {
            Vector3 playerPositionAddition = playerHorizontalMotion;
            playerPositionAddition *= Leggable.Run();
            playerPositionAddition *= Time.deltaTime;
            rigidbody.MovePosition(rigidbody.position + playerPositionAddition); // Movement by foot with AddForce is buggy, so for now this will work.
        }
        else
        {
            Vector3 horizontalThrustersForce = playerHorizontalMotion;
            horizontalThrustersForce *= spaceSuit.FireHorizontalThrusters();
            horizontalThrustersForce *= Time.deltaTime;
            rigidbody.AddForce(horizontalThrustersForce);
        }

        // Fire vertical thrusters
        if (!Mathf.Approximately(playerInput.movement.y, 0f))
        {
            bool useSuperFuel = playerInput.movement.y > 0f && playerInput.jump;

            Vector3 verticalThrustersForce = playerVerticalMotion;
            verticalThrustersForce *= spaceSuit.FireVerticalThrusters(useSuperFuel);
            verticalThrustersForce *= Time.deltaTime;

            rigidbody.AddForce(verticalThrustersForce);
        }

        // Handle jump logic
        if (leggable.IsGrounded())
        {
            if (playerInput.jump)
            {
                jumpable.AccumulateJumpPower();
            }
            else if (!playerInput.jump && jumpable.ReadyToJump)
            {
                Vector3 jumpMotion = transform.up;
                jumpMotion *= jumpable.Jump();
                rigidbody.AddForce(jumpMotion); // There's no Time.deltaTime, because it's a single force push
            }
        }
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    }

    private void PilotSpaceShip()
    {
        buckledUpSpaceShipSeat.spaceShipInterface.PilotShip(playerInput.movement, playerInput.rotation, playerInput.alternativeRotate);
    }

    private void BreatheOxygen()
    {
        hasSomethingToBreathe = spaceSuit.GiveOxygenToBreathe();

        if (!hasSomethingToBreathe)
        {
            // Todo: add little delay (player can survive without oxygen for 30 seconds or so)
            Die();
        }
    }

    private void Rotate()
    {
        if (IsBuckledUp())
        {
            return;
        }

        float horizontalMouseOffset = playerInput.rotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
        float verticalMouseOffset = playerInput.rotation.y * GameSettings.MouseSensitivity * Time.deltaTime;

        headVerticalRotation -= verticalMouseOffset;
        headVerticalRotation = Mathf.Clamp(headVerticalRotation, -90f, 90f); // We don't want our player to roll over with the camera :)
        camera.transform.localRotation = Quaternion.Euler(headVerticalRotation, 0f, 0f);

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

    public void BuckleUpIntoSpaceShipSeat(SpaceShipSeat spaceShipSeat)
    {
        buckledUpSpaceShipSeat = spaceShipSeat;
        buckledUpSpaceShipSeat.StartBucklingUp(this);
    }

    public void UnbuckleFromSpaceShipSeat()
    {
        buckledUpSpaceShipSeat.Unbuckle();
        buckledUpSpaceShipSeat = null;
    }

    public bool IsBuckledUp()
    {
        return buckledUpSpaceShipSeat != null;
    }
}
