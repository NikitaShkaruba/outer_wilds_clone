using Common;
using PlayerTools.SpaceShipParts;
using UnityEngine;
using Universe;

namespace PlayerTools
{
    [RequireComponent(typeof(SpaceShipThrusters))]
    [RequireComponent(typeof(SpaceShipFlashlight))]
    public class SpaceShip : SpaceBody
    {
        [SerializeField] private GameObject hatchRotator;
        [SerializeField] private GameObject hatchGravityField;
        [SerializeField] private SpaceShipAccelerationShowcase accelerationShowcase;
        private SpaceShipThrusters thrusters;

        // Player Input
        private Vector3 wantedMovement;
        private Vector3 wantedRotation;
        private bool wantsToRotateAroundForwardVector;

        // Movement
        [SerializeField] private float movementThrustersPower;
        [SerializeField] private float rotationThrustersPower;

        // Hatch
        [HideInInspector] public bool isHatchClosed;
        private bool isHatchMoving;

        // Flashlight
        private SpaceShipFlashlight flashlight;

        private new void Awake()
        {
            base.Awake();

            thrusters = GetComponent<SpaceShipThrusters>();
            flashlight = GetComponent<SpaceShipFlashlight>();
        }

        private void Update()
        {
            accelerationShowcase.UpdateAcceleration(wantedMovement);
            thrusters.Fire(wantedMovement);
        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
            ApplyGravity();

            if (isHatchMoving)
            {
                MoveHatch();
            }
        }

        public void Pilot(Vector3 playerWantedMovement, Vector2 playerWantedRotation, bool playerWantsToRotateAroundForwardVector)
        {
            wantedMovement = playerWantedMovement;
            wantedRotation = playerWantedRotation;
            wantsToRotateAroundForwardVector = playerWantsToRotateAroundForwardVector;
        }

        private void Move()
        {
            Transform cachedTransform = transform;

            // Get direction
            Vector3 motion = cachedTransform.forward * wantedMovement.x +
                             cachedTransform.up * wantedMovement.y +
                             cachedTransform.right * wantedMovement.z;

            motion *= movementThrustersPower; // Add power
            motion *= Time.deltaTime; // Add physics step

            rigidbody.AddForce(motion);
        }

        private void Rotate()
        {
            Vector2 rotation = wantedRotation; // Get direction
            rotation *= rotationThrustersPower; // Add more force
            rotation *= GameSettings.MouseSensitivity; // Apply mouse sensitivity settings
            rotation *= Time.deltaTime; // Include physics step

            rigidbody.AddTorque(transform.right * -rotation.y);

            if (wantsToRotateAroundForwardVector)
            {
                rigidbody.AddTorque(transform.forward * -rotation.x);
            }
            else
            {
                rigidbody.AddTorque(transform.up * rotation.x);
            }
        }

        public void PlayerEnteredShip()
        {
            if (isHatchClosed || !hatchGravityField.activeSelf)
            {
                return;
            }

            InitializeHatchMoving();
        }

        public void PlayerExitedShip()
        {
            hatchGravityField.SetActive(true);
        }

        public void OpenHatch()
        {
            if (!isHatchClosed)
            {
                return;
            }

            InitializeHatchMoving();
        }

        private void InitializeHatchMoving()
        {
            isHatchMoving = true;
        }

        private void MoveHatch()
        {
            float hatchClosingSpeed = 300f;
            hatchClosingSpeed *= isHatchClosed ? 1 : -1;

            hatchRotator.transform.Rotate(hatchClosingSpeed * Time.deltaTime, 0f, 0f);

            float finalXEuler = isHatchClosed ? -90 : 90;
            Quaternion desiredRotation = Quaternion.Euler(finalXEuler, 0, 0);

            float angleDifference = Quaternion.Angle(hatchRotator.transform.localRotation, desiredRotation);
            if (Mathf.Approximately(angleDifference, 0))
            {
                FinishHatchMoving();
            }
        }

        private void FinishHatchMoving()
        {
            isHatchMoving = false;
            isHatchClosed = !isHatchClosed;

            if (isHatchClosed)
            {
                hatchGravityField.SetActive(false);
            }
        }

        public void ToggleFlashlight()
        {
            flashlight.Toggle();
        }
    }
}
