using UnityEngine;

namespace Physics
{
    public static class Gravitation
    {
        private const float GravitationalConstant = 50000000f;

        public static Vector3 ComputeCelestialBodyForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            Vector3 force = ComputeNewtonsForce(firstBody, secondBody);

            // Hack Newton's law in order to increase orbital speed with mass.
            // Maybe we should add orbital speed multiplier???
            force /= firstBody.mass;

            return force;
        }

        public static Vector3 ComputeNonCelestialBodyForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            Vector3 force = ComputeNewtonsForce(firstBody, secondBody);

            // Apply less gravity if the celestial body is far away
            Vector3 positionsDifference = secondBody.position - firstBody.position;
            force /= 10 * positionsDifference.magnitude;

            return force;
        }

        private static Vector3 ComputeNewtonsForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            Vector3 positionsDifference = secondBody.position - firstBody.position;

            // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
            float forceMagnitude = GravitationalConstant *
                                   (firstBody.mass * secondBody.mass / positionsDifference.sqrMagnitude);

            // Add direction
            return positionsDifference.normalized * forceMagnitude;
        }
    }
}
