using System.Collections.Generic;
using Common;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipThrusters : MonoBehaviour
    {
        // Player input
        private Vector3 wantedAcceleration;
        private Vector3 wantedRotation;
        private bool rotateAlternatively;

        // Movement
        public SpaceShip spaceShip;
        [SerializeField] private float movementThrustersPower = 300000;
        [SerializeField] private float rotationThrustersPower = 500;

        // Thruster fire effect blocks
        private Dictionary<string, Dictionary<string, ParticleSystem>> thrusterFireEffects;
        [SerializeField] private ParticleSystem leftTopFireEffect;
        [SerializeField] private ParticleSystem leftFrontFireEffect;
        [SerializeField] private ParticleSystem leftBottomFireEffect;
        [SerializeField] private ParticleSystem leftBackFireEffect;
        [SerializeField] private ParticleSystem leftLeftFireEffect;
        [SerializeField] private ParticleSystem rightTopFireEffect;
        [SerializeField] private ParticleSystem rightFrontFireEffect;
        [SerializeField] private ParticleSystem rightBottomFireEffect;
        [SerializeField] private ParticleSystem rightBackFireEffect;
        [SerializeField] private ParticleSystem rightRightFireEffect;

        public void Awake()
        {
            InitializeFireEffects();
        }

        private void Update()
        {
            UpdateFireEffects();
        }

        private void FixedUpdate()
        {
            MoveShip();
            RotateShip();
        }

        public void UpdateInput(Vector3 playerWantedAcceleration, Vector2 playerWantedRotation, bool playerWantsToRotateAlternatively)
        {
            wantedAcceleration = playerWantedAcceleration;
            wantedRotation = playerWantedRotation;
            rotateAlternatively = playerWantsToRotateAlternatively;
        }

        private void MoveShip()
        {
            Transform cachedTransform = transform;

            // Get direction
            Vector3 motion = cachedTransform.forward * wantedAcceleration.x +
                             cachedTransform.up * wantedAcceleration.y +
                             cachedTransform.right * wantedAcceleration.z;

            motion *= movementThrustersPower; // Add power
            motion *= Time.deltaTime; // Add physics step

            spaceShip.rigidbody.AddForce(motion);
        }

        private void RotateShip()
        {
            Vector2 rotation = wantedRotation; // Get direction
            rotation *= rotationThrustersPower; // Add more force
            rotation *= GameSettings.MouseSensitivity; // Apply mouse sensitivity settings
            rotation *= Time.deltaTime; // Include physics step

            spaceShip.rigidbody.AddTorque(transform.right * -rotation.y);

            if (rotateAlternatively)
            {
                spaceShip.rigidbody.AddTorque(transform.forward * -rotation.x);
            }
            else
            {
                spaceShip.rigidbody.AddTorque(transform.up * rotation.x);
            }
        }

        private void InitializeFireEffects()
        {
            thrusterFireEffects = new Dictionary<string, Dictionary<string, ParticleSystem>>
            {
                [Directions.Left] = new Dictionary<string, ParticleSystem>(),
                [Directions.Right] = new Dictionary<string, ParticleSystem>(),

                [Directions.Left] = {[Directions.Top] = leftTopFireEffect},
                [Directions.Left] = {[Directions.Front] = leftFrontFireEffect},
                [Directions.Left] = {[Directions.Bottom] = leftBottomFireEffect},
                [Directions.Left] = {[Directions.Back] = leftBackFireEffect},
                [Directions.Left] = {[Directions.Left] = leftLeftFireEffect},

                [Directions.Right] = {[Directions.Top] = rightTopFireEffect},
                [Directions.Right] = {[Directions.Front] = rightFrontFireEffect},
                [Directions.Right] = {[Directions.Bottom] = rightBottomFireEffect},
                [Directions.Right] = {[Directions.Back] = rightBackFireEffect},
                [Directions.Right] = {[Directions.Right] = rightRightFireEffect}
            };
        }

        private void UpdateFireEffects()
        {
            bool isAcceleratingForward = wantedAcceleration.x > 0;
            UpdateFireEffect(Directions.Left, Directions.Back, isAcceleratingForward);
            UpdateFireEffect(Directions.Right, Directions.Back, isAcceleratingForward);

            bool isAcceleratingBackwards = wantedAcceleration.x < 0;
            UpdateFireEffect(Directions.Left, Directions.Front, isAcceleratingBackwards);
            UpdateFireEffect(Directions.Right, Directions.Front, isAcceleratingBackwards);

            bool isAcceleratingUp = wantedAcceleration.y > 0;
            UpdateFireEffect(Directions.Left, Directions.Bottom, isAcceleratingUp);
            UpdateFireEffect(Directions.Right, Directions.Bottom, isAcceleratingUp);

            bool isAcceleratingDown = wantedAcceleration.y < 0;
            UpdateFireEffect(Directions.Left, Directions.Top, isAcceleratingDown);
            UpdateFireEffect(Directions.Right, Directions.Top, isAcceleratingDown);

            bool isAcceleratingRight = wantedAcceleration.z > 0;
            UpdateFireEffect(Directions.Left, Directions.Left, isAcceleratingRight);

            bool isAcceleratingLeft = wantedAcceleration.z < 0;
            UpdateFireEffect(Directions.Right, Directions.Right, isAcceleratingLeft);
        }

        private void UpdateFireEffect(string thrusterBlockDirection, string direction, bool active)
        {
            ParticleSystem fireEffect = thrusterFireEffects[thrusterBlockDirection][direction];

            if (fireEffect.isPlaying && !active)
            {
                fireEffect.Stop();
            }
            else if (!fireEffect.isPlaying && active)
            {
                fireEffect.Play();
            }
        }
    }
}
