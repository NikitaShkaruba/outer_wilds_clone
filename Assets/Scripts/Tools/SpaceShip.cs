using UnityEngine;

namespace Tools
{
    public class SpaceShip : Body
    {
        [SerializeField] private GameObject hatchRotator;
        [SerializeField] private GameObject hatchGravityField;

        // Player Input
        private Vector3 wantedMovement;
        private Vector3 wantedRotation;
        private bool wantsToRotateAroundForwardVector;

        // Movement
        private const float MoveThrustersAcceleration = 400000f;
        private const float RotationThrustersAcceleration = 1500f;

        // Hatch
        public bool isHatchClosed;
        private bool isHatchMoving;

        private void FixedUpdate()
        {
            Move();
            Rotate();

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

            motion *= MoveThrustersAcceleration; // Add power
            motion *= Time.deltaTime; // Add physics step           

            rigidbody.AddForce(motion);
        }

        private void Rotate()
        {
            Vector2 rotation = wantedRotation; // Get direction
            rotation *= RotationThrustersAcceleration; // Add more force
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

            float finalXEuler = isHatchClosed ? 180 : 0;
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
    }
}