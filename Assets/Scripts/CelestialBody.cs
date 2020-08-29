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
        
        rigidbody.AddForce(initialVelocity * 1000);
    }

    private Orbit CreateOrbit()
    {
        Color color;
        switch (name)
        {
            case "Ash Twin": color = Color.yellow; break;
            case "Ember Twin": color = Color.red; break;
            case "Timber Hearth": color = Color.green; break;
            default: color = Color.white; break;
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
        bool isCloseEnough = Mathf.Abs(Position.x) < 10f || Mathf.Abs(Position.z) < 10f;
        if (name == "Ember Twin" && isCloseEnough)
        {
            Debug.Log($"Ember Twin EDGE. Coordinates {Position.x}, {Position.z}. Time: {Time.time}");
        }
        if (name == "Ash Twin" && isCloseEnough)
        {
            Debug.Log($"Ash Twin EDGE. Coordinates {Position.x}, {Position.z}. Time: {Time.time}");
        }
        if (name == "Timber Hearth" && isCloseEnough)
        {
            Debug.Log($"Timber Hearth EDGE. Coordinates {Position.x}, {Position.z}. Time: {Time.time}");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Debug. Helps to find the perfect numbers for Hourglass Twins. Will need it until the whole solar system is done
        Debug.Log("COLLISION");
    }

    public void ApplyGravity(Vector3 gravityForce)
    {
        rigidbody.AddForce(gravityForce);
    }
}