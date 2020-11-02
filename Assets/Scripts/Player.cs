using System.Linq;
using Celestial;
using Physics;
using PlayerLogic;
using PlayerTools;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpaceSuit))]
[RequireComponent(typeof(MarshmallowCookable))]
public class Player : AcceleratedMonoBehaviour
{
    // Unity components
    public new Camera camera;
    public new Transform transform;
    public new Rigidbody rigidbody;

    // MonoBehaviour humble object components
    public SpaceSuit spaceSuit;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public MarshmallowCookable marshmallowCookable;

    // Other humble object components
    private OxygenBreathable oxygenBreathable;
    private Moveable moveable;
    private Rotatable rotatable;
    public Damageable Damageable;
    public Dieable Dieable;
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

        moveable = new Moveable(this);
        rotatable = new Rotatable();
        gravitatable = new Gravitatable(rigidbody, FindObjectsOfType<CelestialBody>().ToArray());
        towardsCelestialBodyRotatable = new TowardsCelestialBodyRotatable(rigidbody);
        Damageable = new Damageable(100f);
        Dieable = new Dieable();
        BuckledUppable = new BuckledUppable(this);
        oxygenBreathable = new OxygenBreathable();

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

        moveable.Move(playerInput);

        if (!BuckledUppable.IsBuckledUp())
        {
            rotatable.Rotate(transform, camera, playerInput);
        }
    }
}
