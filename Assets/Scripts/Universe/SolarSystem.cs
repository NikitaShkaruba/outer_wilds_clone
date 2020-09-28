using UnityEngine;

namespace Universe
{
    public class SolarSystem : MonoBehaviour
    {
        private const float CelestialBodyGravitationalConstant = 1000000f;
        private const float SpaceBodyGravitationalConstant = 5000f;

        public static CelestialBody[] CelestialBodies;

        private void Awake()
        {
            CelestialBodies = FindObjectsOfType<CelestialBody>();
        }

        public static Vector3 ComputeCelestialBodyGravitationalForce(CelestialBody firstBody, CelestialBody secondBody)
        {
            return ComputeGravitationalForce(firstBody, secondBody, CelestialBodyGravitationalConstant);
        }

        /**
         * I need separate gravitational constant for space bodies to compensate my universe not ideal planet masses, planet velocities.
         * It's easier for me to tweak this once here than to pursue a possible unattainable ideal :)
         */
        public static Vector3 ComputeSpaceBodyGravitationalForce(SpaceBody spaceBody, CelestialBody celestialBody)
        {
            Vector3 gravitationalForce = ComputeGravitationalForce(spaceBody, celestialBody, SpaceBodyGravitationalConstant);

            gravitationalForce *= celestialBody.spaceBodiesGravityScale;
            gravitationalForce *= spaceBody.gravityScale;

            return gravitationalForce;
        }

        private static Vector3 ComputeGravitationalForce(Body firstBody, Body secondBody, float gravitationalConstant)
        {
            Vector3 positionsDifference = secondBody.Position - firstBody.Position;

            // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
            float forceMagnitude = gravitationalConstant *
                                   (firstBody.Mass * secondBody.Mass / positionsDifference.sqrMagnitude);

            // Add direction
            Vector3 force = positionsDifference.normalized * forceMagnitude;

            // Hack Newton's law in order to increase orbital rotation with mass
            force /= firstBody.Mass;

            return force;
        }
    }
}
