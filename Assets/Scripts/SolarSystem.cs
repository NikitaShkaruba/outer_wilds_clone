using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private const float GravitationalConstant = 10000f;

    private CelestialBody[] celestialBodies;

    private void Awake()
    {
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

                Vector3 velocity = ComputeGravitationalForce(celestialBody, otherCelestialBody);

                celestialBody.ApplyGravity(velocity * Time.deltaTime);
            }
        }
    }

    private static Vector3 ComputeGravitationalForce(CelestialBody celestialBody, CelestialBody otherCelestialBody)
    {
        // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
        Vector3 differenceWithOtherBody = otherCelestialBody.Position - celestialBody.Position;
        float destinationSquare = differenceWithOtherBody.sqrMagnitude;
        Vector3 forceDirection = differenceWithOtherBody.normalized;
        Vector3 force = forceDirection * GravitationalConstant *
                        (celestialBody.Mass * otherCelestialBody.Mass / destinationSquare);
        
        return force / celestialBody.Mass;
    }
}