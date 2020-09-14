using UnityEngine;

public class CelestialBody : Body
{
    public new string name;
    public float radius; // Is needed to compute size of a planet. I can somehow get this data from renderers, but for now this will do

    // I want the Sun to always be at 0, 0, 0. I can do it with moving sun, but it will ease the numbers
    public bool isStationary;

    // Nested objects
    private Orbit orbit;

    private new void Awake()
    {
        base.Awake();

        orbit = new Orbit(rigidbody.position, Color.white);
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        DrawOrbit();
    }

    private void ApplyGravity()
    {
        foreach (CelestialBody otherCelestialBody in SolarSystem.CelestialBodies)
        {
            // Don't add force to itself
            if (this == otherCelestialBody)
            {
                continue;
            }

            if (isStationary)
            {
                continue;
            }

            Vector3 gravityForce = SolarSystem.ComputeGravitationalForce(this, otherCelestialBody);
            rigidbody.AddForce(gravityForce); // Todo: add multiplication with Time.deltaTime
        }
    }

    private void DrawOrbit()
    {
        if (isStationary)
        {
            return;
        }

        orbit.Draw();
        orbit.Update(this);
    }
}
