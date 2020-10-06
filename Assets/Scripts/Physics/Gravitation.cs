using UnityEngine;

namespace Physics
{
    public static class Gravitation
    {
        private const float GravitationalConstant = 1000000f;

        public static Vector3 ComputePlayerForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            return ComputeCelestialBodyForce(firstBody, secondBody);
        }

        public static Vector3 ComputeCelestialBodyForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            Vector3 force = ComputeNewtonsForce(firstBody, secondBody);

            // Hack Newton's law in order to increase orbital speed with mass.
            // Todo: Add orbital speed multiplier???
            force /= firstBody.mass;

            return force;
        }

        private static Vector3 ComputeNewtonsForce(Rigidbody firstBody, Rigidbody secondBody)
        {
            Vector3 positionsDifference = secondBody.position - firstBody.position;

            // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
            float forceMagnitude = GravitationalConstant *
                                   (firstBody.mass * secondBody.mass / positionsDifference.sqrMagnitude);

            // Add direction
            Vector3 force = positionsDifference.normalized * forceMagnitude;
            return force;
        }
    }
}
