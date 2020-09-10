using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class GravityField : MonoBehaviour
    {
        private Player pulledPlayer;

        private void FixedUpdate()
        {
            if (pulledPlayer == null)
            {
                return;
            }

            Transform cachedTransform = transform;

            Vector3 force = cachedTransform.position - pulledPlayer.Position; // Pull towards the center
            force += cachedTransform.up * 8f; // Pull up
            force *= 1000f; // Add more power
            force *= Time.deltaTime; // Scale with physics step

            pulledPlayer.rigidbody.AddForce(force);
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }

            pulledPlayer = player;
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