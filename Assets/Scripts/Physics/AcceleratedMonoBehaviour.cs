using UnityEngine;

namespace Physics
{
    /**
     * MonoBehaviour that is accelerated on initialization
     */
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AcceleratedMonoBehaviour : MonoBehaviour
    {
        public Vector3 initialVelocity;

        protected void Awake()
        {
            Rigidbody monoBehaviourRigidbody = GetComponent<Rigidbody>();
            monoBehaviourRigidbody.velocity = initialVelocity;
        }
    }
}
