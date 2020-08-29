using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private const float GravitationalConstant = 1000000f;

    private CelestialBody[] celestialBodies;

    private void Awake()
    {
        Time.timeScale = 100; // Debug. Will need it before the whole solar system is done
        celestialBodies = FindObjectsOfType<CelestialBody>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        foreach (CelestialBody celestialBody in celestialBodies)
        {
            foreach (CelestialBody otherCelestialBody in celestialBodies)
            {
                // Don't add force to itself
                if (celestialBody == otherCelestialBody)
                {
                    continue;
                }

                if (celestialBody.isStationary)
                {
                    continue;
                }

                Vector3 gravityForce = ComputeGravitationalForce(celestialBody, otherCelestialBody);

                celestialBody.ApplyGravity(gravityForce);
            }
        }
    }

    private static Vector3 ComputeGravitationalForce(CelestialBody celestialBody, CelestialBody otherCelestialBody)
    {
        Vector3 positionsDifference = otherCelestialBody.Position - celestialBody.Position;

        // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
        float forceMagnitude = GravitationalConstant *
                               (celestialBody.Mass * otherCelestialBody.Mass / positionsDifference.sqrMagnitude);

        // Add direction
        Vector3 force = positionsDifference.normalized * forceMagnitude;

        return force / celestialBody.Mass;
    }
}