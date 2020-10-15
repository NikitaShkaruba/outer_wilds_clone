using Celestial;
using UnityEngine;

namespace Physics
{
    public class Gravitatable
    {
        private readonly Rigidbody rigidbody;
        private CelestialBody bodyToGravitateTowards;
        private readonly CelestialBody[] celestialBodies;
        private readonly bool rotatable;

        public Gravitatable(Rigidbody rigidbody, CelestialBody[] celestialBodies, bool rotatable = true)
        {
            this.rigidbody = rigidbody;
            this.celestialBodies = celestialBodies;
            this.rotatable = rotatable;
        }

        public void ApplyGravity(bool isNotCelestialBody = false)
        {
            Vector3 maxGravityForce = Vector3.zero;
            bodyToGravitateTowards = null;

            foreach (CelestialBody celestialBody in celestialBodies)
            {
                // This difference is needed because celestial body gravity is a little bit to harsh for the player
                Vector3 gravityForce;
                if (isNotCelestialBody)
                {
                    gravityForce = Vector3.zero;
                    if (celestialBody.name != "Sun")
                    {
                        gravityForce = Gravitation.ComputeNonCelestialBodyForce(rigidbody, celestialBody.rigidbody);
                        gravityForce *= celestialBody.gravityScale;
                    }
                }
                else
                {
                    gravityForce = Gravitation.ComputeCelestialBodyForce(rigidbody, celestialBody.rigidbody);
                }

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
            if (!rotatable)
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
