using Celestial;
using UnityEngine;

namespace Physics
{
    /**
     * Class that rotates a rigidbody towards a celestial body (makes rigidbody's ground to be at the bottom of it's local rotation)
     */
    public class TowardsCelestialBodyRotatable
    {
        private readonly Rigidbody rigidbody;

        private CelestialBody bodyToRotateTowards;
        private Vector3 maxGravityForce = Vector3.zero;

        public TowardsCelestialBodyRotatable(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void RotateIfNeeded(MaxGravitatableInfo maxGravitatableInfo)
        {
            if (!ShouldRotateTowardsCelestialBody(maxGravitatableInfo.MaxGravityForce, maxGravitatableInfo.CelestialBody))
            {
                return;
            }

            RotateTowardsCelestialBody(maxGravitatableInfo.CelestialBody);
        }


        private bool ShouldRotateTowardsCelestialBody(Vector3 gravityForce, CelestialBody celestialBody)
        {
            if (celestialBody == null)
            {
                return false;
            }

            // We only rotate to a body with the most gravity force
            if (gravityForce.magnitude < maxGravityForce.magnitude)
            {
                return false;
            }

            // We only rotate to a body if it is nearby
            if ((celestialBody.rigidbody.position - rigidbody.position).magnitude > 600f)
            {
                return false;
            }

            // We don't rotate to the sun, because it's impossible to land on it
            if (celestialBody.name == "Sun")
            {
                return false;
            }

            return true;
        }

        private void RotateTowardsCelestialBody(CelestialBody celestialBody)
        {
            Transform cachedTransform = rigidbody.transform;
            Quaternion cachedTransformRotation = cachedTransform.rotation;

            Vector3 gravityForceDirection = (cachedTransform.position - celestialBody.rigidbody.position).normalized;
            Vector3 playerUp = cachedTransform.up;
            Quaternion neededRotation = Quaternion.FromToRotation(playerUp, gravityForceDirection) * cachedTransformRotation;

            cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, neededRotation, Time.deltaTime);
            cachedTransform.rotation = cachedTransformRotation;
        }
    }
}
