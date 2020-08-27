using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private const float GravitationalConstant = 1f;

    private CelestialBody[] celestialBodies;
    private float deltaTime = 1;

    // Celestial body trajectories
    [SerializeField] private bool showTrajectories;
    [SerializeField] private int trajectorySteps = 100;
    private Vector3[][] celestialBodiesTrajectories = null;

    private void Awake()
    {
        celestialBodies = FindObjectsOfType<CelestialBody>();
    }

    private void FixedUpdate()
    {

        foreach (CelestialBody celestialBody in celestialBodies)
        {
            Vector3 velocity = ComputeCelestialBodyVelocity(celestialBody, celestialBodies);

            celestialBody.AddVelocity(velocity * deltaTime);
            celestialBody.UpdatePosition();
        }

        if (showTrajectories)
        {
            DrawTrajectories();
        }
    }

    private static Vector3 ComputeCelestialBodyVelocity(CelestialBody celestialBody, CelestialBody[] celestialBodies)
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

    private void DrawTrajectories()
    {
        if (celestialBodiesTrajectories == null)
        {
            ComputeTrajectories();
        }

        DrawTrajectoriesInternal();
    }

    private void ComputeTrajectories()
    {
        var random = new System.Random();
        if (celestialBodiesTrajectories != null && random.Next(0, 10) != 1)
        {
            return;
        }
        
        // Initialize celestial bodies copy
        CelestialBody[] celestialBodiesCopy = new CelestialBody[celestialBodies.Length];
        for (int celestialBodyIndex = 0; celestialBodyIndex < celestialBodies.Length; celestialBodyIndex++)
        {
            CelestialBody celestialBodyCopy = Instantiate(celestialBodies[celestialBodyIndex]);
            celestialBodiesCopy[celestialBodyIndex] = celestialBodyCopy;
        }

        // Initialize trajectories
        celestialBodiesTrajectories = new Vector3[celestialBodiesCopy.Length][];
        for (int celestialBodyIndex = 0; celestialBodyIndex < celestialBodiesCopy.Length; celestialBodyIndex++)
        {
            celestialBodiesTrajectories[celestialBodyIndex] = new Vector3[trajectorySteps];
        }

        // Compute steps
        for (int trajectoryIndex = 0; trajectoryIndex < trajectorySteps; trajectoryIndex++)
        {
            for (int celestialBodyIndex = 0; celestialBodyIndex < celestialBodiesCopy.Length; celestialBodyIndex++)
            {
                CelestialBody celestialBody = celestialBodiesCopy[celestialBodyIndex];
                Vector3 celestialBodyVelocity = ComputeCelestialBodyVelocity(celestialBody, celestialBodiesCopy);
                celestialBody.AddVelocity(celestialBodyVelocity * deltaTime);
                celestialBody.UpdatePosition();
                celestialBodiesTrajectories[celestialBodyIndex][trajectoryIndex] = new Vector3(
                    celestialBody.rigidbody.position.x, celestialBody.rigidbody.position.y,
                    celestialBody.rigidbody.position.z);
            }
        }

        // Cleanup
        foreach (CelestialBody celestialBody in celestialBodiesCopy)
        {
            DestroyImmediate(celestialBody);
        }
        
        Debug.Log("Updated Trajectories");
    }

    private void DrawTrajectoriesInternal()
    {
        for (int celestialBodyIndex = 0; celestialBodyIndex < celestialBodiesTrajectories.Length; celestialBodyIndex++)
        {
            Color lineColor = celestialBodyIndex == 0 ? Color.white : Color.yellow;
                
            for (int trajectoryIndex = 0; trajectoryIndex < celestialBodiesTrajectories[celestialBodyIndex].Length - 1; trajectoryIndex++)
            {
                Vector3 start = celestialBodiesTrajectories[celestialBodyIndex][trajectoryIndex];
                Vector3 end = celestialBodiesTrajectories[celestialBodyIndex][trajectoryIndex + 1];
                
                Debug.DrawLine(start, end, lineColor);
            }
        }
    }
}