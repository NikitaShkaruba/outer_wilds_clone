using UnityEngine;

namespace Universe
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Body : MonoBehaviour
    {
        [HideInInspector] public new Rigidbody rigidbody;

        public Vector3 Position => rigidbody.position;
        public float Mass => rigidbody.mass;
        public Vector3 initialVelocity;

        protected void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = initialVelocity;
        }
    }
}