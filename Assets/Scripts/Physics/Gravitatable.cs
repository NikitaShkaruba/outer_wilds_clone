using UnityEngine;
using Universe;

namespace Physics
{
    public class Gravitatable
    {
        private readonly Rigidbody rigidbody;
        private CelestialBody bodyToGravitateTowards;

        public Gravitatable(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void ApplyGravity(float multiplier = 1f)
        {
            Vector3 maxGravityForce = Vector3.zero;
            bodyToGravitateTowards = null;

            foreach (CelestialBody celestialBody in SolarSystem.CelestialBodies)
            {
                Vector3 gravityForce = Gravitation.ComputePlayerForce(rigidbody, celestialBody.rigidbody);
                gravityForce *= celestialBody.spaceBodiesGravityScale;
                gravityForce *= multiplier;
                gravityForce *= Time.deltaTime;
                rigidbody.AddForce(gravityForce);

                if (ShouldRotateTowardsCelestialBody(gravityForce, maxGravityForce, celestialBody))
                {
                    maxGravityForce = gravityForce;
                    bodyToGravitateTowards = celestialBody;
                }
            }

            if (bodyToGravitateTowards != null)
            {
                RotateTowardsCelestialBody(bodyToGravitateTowards);
            }
        }

        private bool ShouldRotateTowardsCelestialBody(Vector3 gravityForce, Vector3 maxGravityForce, CelestialBody celestialBody)
        {
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
