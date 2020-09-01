using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public static CelestialBody[] CelestialBodies;

    private void Awake()
    {
        CelestialBodies = FindObjectsOfType<CelestialBody>();
    }

    public static Vector3 ComputeGravitationalForce(Body firstBody, Body secondBody)
    {
        const float gravitationalConstant = 1000000f;
        
        Vector3 positionsDifference = secondBody.Position - firstBody.Position;

        // Newton's law of universal gravitation F = G * (m1 * m2 / r^2)
        float forceMagnitude = gravitationalConstant *
                               (firstBody.Mass * secondBody.Mass / positionsDifference.sqrMagnitude);

        // Add direction
        Vector3 force = positionsDifference.normalized * forceMagnitude;

        return force / firstBody.Mass;
    }
}