using UnityEngine;

public class SpaceBody : Body
{
    protected CelestialBody BodyToGravitateTowards;

    protected void ApplyGravity()
    {
        Vector3 maxGravityForce = Vector3.zero;
        BodyToGravitateTowards = null;

        foreach (CelestialBody celestialBody in SolarSystem.CelestialBodies)
        {
            Vector3 gravityForce = SolarSystem.ComputeGravitationalForce(this, celestialBody) / 50f; // Todo: do something with this number
            rigidbody.AddForce(gravityForce * Time.deltaTime);

            if (ShouldRotateTowardsCelestialBody(gravityForce, maxGravityForce, celestialBody))
            {
                maxGravityForce = gravityForce;
                BodyToGravitateTowards = celestialBody;
            }
        }

        if (BodyToGravitateTowards != null)
        {
            RotateTowardsCelestialBody(BodyToGravitateTowards);
        }
    }

    private bool ShouldRotateTowardsCelestialBody(Vector3 gravityForce, Vector3 maxGravityForce, CelestialBody celestialBody)
    {
        // We only rotate to a body with the most gravity force
        if (gravityForce.magnitude < maxGravityForce.magnitude)
        {
            return false;
        }

        // We only rotate to a body if it is nearby
        if ((celestialBody.Position - Position).magnitude > 600f)
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
        Transform cachedTransform = transform;
        Quaternion cachedTransformRotation = cachedTransform.rotation;

        Vector3 gravityForceDirection = (cachedTransform.position - celestialBody.Position).normalized;
        Vector3 playerUp = cachedTransform.up;
        Quaternion neededRotation = Quaternion.FromToRotation(playerUp, gravityForceDirection) * cachedTransformRotation;

        cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, neededRotation, Time.deltaTime);
        cachedTransform.rotation = cachedTransformRotation;
    }
}
