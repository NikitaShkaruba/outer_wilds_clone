using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private const float GravitationalConstant = 1000000f;

    private CelestialBody[] celestialBodies;
    private Player player;

    private void Awake()
    {
        celestialBodies = FindObjectsOfType<CelestialBody>();
        player = FindObjectOfType<Player>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        player.ResetRotation();
        
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

            Vector3 playerGravityForce = ComputeGravitationalForce(player, celestialBody) / 1200f; // 400f just for now. I don't understand why it works
            player.ApplyGravity(playerGravityForce);
            player.RememberRotation(playerGravityForce, celestialBody);
        }
        
        player.RotateTowardsGravity();
    }

    private static Vector3 ComputeGravitationalForce(Body firstBody, Body secondBody)
    {
        Vector3 positionsDifference = secondBody.Position - firstBody.Position;

        // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
        float forceMagnitude = GravitationalConstant *
                               (firstBody.Mass * secondBody.Mass / positionsDifference.sqrMagnitude);

        // Add direction
        Vector3 force = positionsDifference.normalized * forceMagnitude;

        return force / firstBody.Mass;
    }
}