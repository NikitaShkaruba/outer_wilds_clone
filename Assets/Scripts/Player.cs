using System.Linq;
using Celestial;
using Physics;
using PlayerLogic;
using PlayerTools;
using UI.Debug;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpaceSuit))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarshmallowCookable))]
public class Player : AcceleratedMonoBehaviour
{
    // Unity components
    public new Camera camera;
    private new Transform transform;
    public new Rigidbody rigidbody;

    // MonoBehaviour humble object components
    public SpaceSuit spaceSuit;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public MarshmallowCookable marshmallowCookable;

    // Other humble object components
    private OxygenBreathable oxygenBreathable;
    private Rotatable rotatable;
    public Damageable Damageable;
    public Dieable Dieable;
    private Leggable leggable;
    private Jumpable jumpable;
    private Gravitatable gravitatable;
    private TowardsCelestialBodyRotatable towardsCelestialBodyRotatable;
    public BuckledUppable BuckledUppable;

    public new void Awake()
    {
        base.Awake();

        // Internal unity components
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();

        // Separate components
        playerInput = GetComponent<PlayerInput>();
        spaceSuit = GetComponent<SpaceSuit>();
        marshmallowCookable = GetComponent<MarshmallowCookable>();

        gravitatable = new Gravitatable(rigidbody, FindObjectsOfType<CelestialBody>().ToArray());
        towardsCelestialBodyRotatable = new TowardsCelestialBodyRotatable(rigidbody);
        Damageable = new Damageable(100f);
        leggable = new Leggable(this);
        jumpable = new Jumpable(this);
        BuckledUppable = new BuckledUppable(this);
        Dieable = new Dieable();
        oxygenBreathable = new OxygenBreathable();
        rotatable = new Rotatable();

        Damageable.OnNoHealthPointsRemaining += Dieable.Die;
        oxygenBreathable.OnNoOxygenLeft += Dieable.Die;
    }

    public void OnDestroy()
    {
        Damageable.OnNoHealthPointsRemaining -= Dieable.Die;
        oxygenBreathable.OnNoOxygenLeft -= Dieable.Die;
    }

    private void FixedUpdate()
    {
        oxygenBreathable.BreatheOxygen(spaceSuit);

        if (BuckledUppable.IsBuckledUp())
        {
            BuckledUppable.PilotShip(playerInput.movement, playerInput.rotation, playerInput.alternativeRotate);
            return;
        }

        if (marshmallowCookable.IsCooking())
        {
            marshmallowCookable.HoldMarshmallowStick(playerInput.rotation, playerInput.extendMarshmallowStick);
            return;
        }

        MaxGravitatableInfo maxGravitatableInfo = gravitatable.ApplyGravity();
        towardsCelestialBodyRotatable.RotateIfNeeded(maxGravitatableInfo);

        if (Dieable.IsDead)
        {
            return;
        }

        Move();

        if (!BuckledUppable.IsBuckledUp())
        {
            rotatable.Rotate(transform, camera, playerInput);
        }

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
            // Movement by foot with AddForce is buggy, so for now this will work.
            // 03 November 2020 Update: Should've used AddForce :D
            rigidbody.MovePosition(rigidbody.position + playerPositionAddition);
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
}
