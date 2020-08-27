using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [HideInInspector] public new Rigidbody rigidbody;

    // Movement
    public float mass;
    [SerializeField] private Vector3 initialVelocity;
    private Vector3 currentVelocity;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        currentVelocity = initialVelocity;
    }

    public void AddVelocity(Vector3 velocity)
    {
        currentVelocity += velocity;
    }

    public void UpdatePosition()
    {
        rigidbody.position += currentVelocity;
    }
}