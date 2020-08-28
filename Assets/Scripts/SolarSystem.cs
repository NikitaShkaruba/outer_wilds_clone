using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private const float GravitationalConstant = 10f;

    private CelestialBody[] celestialBodies;

    private void Awake()
    {
        celestialBodies = FindObjectsOfType<CelestialBody>();
    }

    private void FixedUpdate()
    {

        foreach (CelestialBody celestialBody in celestialBodies)
        {
            Vector3 velocity = ComputeCelestialBodyVelocity(celestialBody);

            celestialBody.AddVelocity(velocity * Time.deltaTime);
            celestialBody.UpdatePosition();
        }
    }

    private Vector3 ComputeCelestialBodyVelocity(CelestialBody celestialBody)
    {
        Vector3 velocity = Vector3.zero;

        foreach (CelestialBody otherCelestialBody in celestialBodies)
        {
            // Don't add force to itself
            if (celestialBody == otherCelestialBody)
            {
                continue;
            }

            // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
            Vector3 differenceWithOtherBody = otherCelestialBody.rigidbody.position - celestialBody.rigidbody.position;
            float destinationSquare = differenceWithOtherBody.sqrMagnitude;
            Vector3 forceDirection = differenceWithOtherBody.normalized;
            Vector3 force = forceDirection * GravitationalConstant *
                            (celestialBody.mass * otherCelestialBody.mass / destinationSquare);
            Vector3 acceleration = force / celestialBody.mass;

            velocity += acceleration;
        }

        return velocity;
    }
}