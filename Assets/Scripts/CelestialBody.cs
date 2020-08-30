using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // Components
    private new Rigidbody rigidbody;

    public new string name;

    // Movement
    public Vector3 initialVelocity;
    public Vector3 Position => rigidbody.position;
    public float Mass => rigidbody.mass;

    // I want the Sun to always be at 0, 0, 0. I can do it with moving sun, but it will ease the numbers
    public bool isStationary;

    // Nested objects
    private Orbit orbit;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        orbit = CreateOrbit();

        rigidbody.velocity = initialVelocity;
    }

    private Orbit CreateOrbit()
    {
        Color color;
        switch (name)
        {
            case "Sun Station":
                color = Color.yellow;
                break;
            case "Ash Twin":
                color = Color.gray;
                break;
            case "Ember Twin":
                color = Color.red;
                break;
            case "Timber Hearth":
                color = Color.green;
                break;
            default:
                color = Color.white;
                break;
        }

        return new Orbit(rigidbody.position, color);
    }

    private void FixedUpdate()
    {
        if (!isStationary)
        {
            orbit.Draw();
            orbit.Update(this);
        }

        // Debug. Will need it until the whole solar system is done
        if (name == "Some Name")
        {
            Debug.Log($"Coordinates {Position.x}, {Position.z}. Time: {Time.time}");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Debug. Helps to find the perfect numbers for Hourglass Twins. Will need it until the whole solar system is done
        Debug.LogError("COLLISION");
    }

    public void ApplyGravity(Vector3 gravityForce)
    {
        rigidbody.AddForce(gravityForce);
    }
}