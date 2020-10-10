﻿using System;
using System.Linq;
using Celestial;
using Common;
using Physics;
using PlayerLogic;
using PlayerTools;
using UI.Debug;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpaceSuit))]
[RequireComponent(typeof(Rigidbody))]
public class Player : AcceleratedMonoBehaviour
{
    // Internal unity components
    public new Camera camera;
    private new Transform transform;
    public new Rigidbody rigidbody;

    // Internal components
    public Damageable Damageable;
    private PlayerInput playerInput;
    private Leggable leggable;
    private Jumpable jumpable;
    private Gravitatable gravitatable;
    public SpaceSuit spaceSuit;
    public BuckledUppable BuckledUppable;

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
        rigidbody = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();
        spaceSuit = GetComponent<SpaceSuit>();

        gravitatable = new Gravitatable(rigidbody, FindObjectsOfType<CelestialBody>().ToArray());
        Damageable = new Damageable(100f);
        leggable = new Leggable(this);
        jumpable = new Jumpable(this);
        BuckledUppable = new BuckledUppable(this);

        Damageable.OnDeath += Die;
    }

    public void OnDestroy()
    {
        Damageable.OnDeath -= Die;
    }

    private void FixedUpdate()
    {
        BreatheOxygen();

        if (BuckledUppable.IsBuckledUp())
        {
            BuckledUppable.PilotShip(playerInput.movement, playerInput.rotation, playerInput.alternativeRotate);
            return;
        }

        gravitatable.ApplyGravity();

        if (isDead)
        {
            return;
        }

        Move();
        Rotate();

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

    private void BreatheOxygen()
    {
        hasSomethingToBreathe = spaceSuit.GiveOxygenToBreathe();

        if (!hasSomethingToBreathe)
        {
            // Maybe add a little delay later (player can survive without oxygen for 30 seconds or so)
            Die();
        }
    }

    private void Rotate()
    {
        if (BuckledUppable.IsBuckledUp())
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
}
