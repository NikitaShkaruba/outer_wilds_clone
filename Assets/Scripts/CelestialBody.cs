using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // Components
    private new Rigidbody rigidbody;
   
    // Movement
    public Vector3 initialVelocity;
    public Vector3 Position => rigidbody.position;
    public float Mass => rigidbody.mass;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(initialVelocity);
    }

    public void ApplyGravity(Vector3 gravityForce)
    {
        rigidbody.AddForce(gravityForce);
    }
}