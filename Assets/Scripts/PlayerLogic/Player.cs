using System.Linq;
using Celestial;
using Physics;
using PlayerTools;
using UnityEngine;

namespace PlayerLogic
{
    /**
     * Player's main MonoBehaviour - which is a gian Humble Object.
     * If you want to understand how player works - check out his humble object components
     */
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerControllable))]
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
        [HideInInspector] public PlayerControllable playerControllable;
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
            playerControllable = GetComponent<PlayerControllable>();
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
                BuckledUppable.PilotShip(playerControllable.movement, playerControllable.rotation, playerControllable.alternativeRotate);
                return;
            }

            if (marshmallowCookable.IsCooking())
            {
                marshmallowCookable.HoldMarshmallowStick(playerControllable.rotation, playerControllable.extendMarshmallowStick);
                return;
            }

            MaxGravitatableInfo maxGravitatableInfo = gravitatable.ApplyGravity();
            towardsCelestialBodyRotatable.RotateIfNeeded(maxGravitatableInfo);

            if (Dieable.IsDead)
            {
                return;
            }

            moveable.Move(playerControllable);

            if (!BuckledUppable.IsBuckledUp())
            {
                rotatable.Rotate(transform, camera, playerControllable);
            }
        }
    }
}
