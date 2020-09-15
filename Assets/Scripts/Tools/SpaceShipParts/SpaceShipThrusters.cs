using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class SpaceShipThrusters : MonoBehaviour
    {
        private Dictionary<string, Dictionary<string, ParticleSystem>> thrusterFireEffects;

        // Left thrusters block
        [SerializeField] private ParticleSystem leftTopFireEffect;
        [SerializeField] private ParticleSystem leftFrontFireEffect;
        [SerializeField] private ParticleSystem leftBottomFireEffect;
        [SerializeField] private ParticleSystem leftBackFireEffect;
        [SerializeField] private ParticleSystem leftLeftFireEffect;

        // Right thrusters block
        [SerializeField] private ParticleSystem rightTopFireEffect;
        [SerializeField] private ParticleSystem rightFrontFireEffect;
        [SerializeField] private ParticleSystem rightBottomFireEffect;
        [SerializeField] private ParticleSystem rightBackFireEffect;
        [SerializeField] private ParticleSystem rightRightFireEffect;

        public void Awake()
        {
            InitializeThrusterBlocks();
        }

        private void InitializeThrusterBlocks()
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

        public void Fire(Vector3 acceleration)
        {
            bool isAcceleratingForward = acceleration.x > 0;
            UpdateFireEffect(Directions.Left, Directions.Back, isAcceleratingForward);
            UpdateFireEffect(Directions.Right, Directions.Back, isAcceleratingForward);

            bool isAcceleratingBackwards = acceleration.x < 0;
            UpdateFireEffect(Directions.Left, Directions.Front, isAcceleratingBackwards);
            UpdateFireEffect(Directions.Right, Directions.Front, isAcceleratingBackwards);

            bool isAcceleratingUp = acceleration.y > 0;
            UpdateFireEffect(Directions.Left, Directions.Bottom, isAcceleratingUp);
            UpdateFireEffect(Directions.Right, Directions.Bottom, isAcceleratingUp);

            bool isAcceleratingDown = acceleration.y < 0;
            UpdateFireEffect(Directions.Left, Directions.Top, isAcceleratingDown);
            UpdateFireEffect(Directions.Right, Directions.Top, isAcceleratingDown);

            bool isAcceleratingRight = acceleration.z > 0;
            UpdateFireEffect(Directions.Left, Directions.Left, isAcceleratingRight);

            bool isAcceleratingLeft = acceleration.z < 0;
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
