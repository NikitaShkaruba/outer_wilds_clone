using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class SpaceShipGravityField : MonoBehaviour
    {
        public float pullPower;

        private void OnTriggerStay(Collider other)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            Transform cachedTransform = transform;

            Vector3 force = cachedTransform.up; // Pull up
            force *= pullPower; // Add power
            force *= Time.fixedDeltaTime; // Scale with physics step

            collidedPlayer.rigidbody.AddForce(force);
        }
    }
}
