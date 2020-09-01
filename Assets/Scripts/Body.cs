using UnityEngine;

public class Body : MonoBehaviour
{
    public new Rigidbody rigidbody;

    public Vector3 Position => rigidbody.position;
    public float Mass => rigidbody.mass;
    public Vector3 initialVelocity;

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = initialVelocity;
    }

    public void ApplyGravity(Vector3 gravityForce)
    {
        rigidbody.AddForce(gravityForce);
    }
}