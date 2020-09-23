using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class SpaceShipGravityField : MonoBehaviour
    {
        private Player pulledPlayer;

        public float pullPower;

        private void FixedUpdate()
        {
            if (pulledPlayer == null)
            {
                return;
            }

            Transform cachedTransform = transform;

            Vector3 force = cachedTransform.up; // Pull up
            force *= pullPower; // Add power
            force *= Time.deltaTime; // Scale with physics step

            pulledPlayer.rigidbody.AddForce(force);
        }

        private void OnTriggerEnter(Collider other)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            pulledPlayer = collidedPlayer;
        }

        private void OnTriggerExit(Collider other)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            pulledPlayer = null;
        }
    }
}
