using UnityEngine;

public class CelestialBody : Body
{
    public new string name;

    // I want the Sun to always be at 0, 0, 0. I can do it with moving sun, but it will ease the numbers
    public bool isStationary;

    // Nested objects
    private Orbit orbit;

    private new void Awake()
    {
        base.Awake();
        
        orbit = CreateOrbit();
    }

    private Orbit CreateOrbit()
    {
        return new Orbit(rigidbody.position, Color.white);
    }

    private void FixedUpdate()
    {
        if (!isStationary)
        {
            orbit.Draw();
            orbit.Update(this);
        }
    }
}