using Cinemachine.Utility;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
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
            Vector3 cachedTransformUp = cachedTransform.up;

            Vector3 positionDifference = cachedTransform.position - collidedPlayer.Position;
            Vector3 directionToCenter = positionDifference.ProjectOntoPlane(cachedTransformUp);

            Vector3 force = directionToCenter; // Pull towards the center
            force += cachedTransformUp; // Pull up
            force *= pullPower; // Add power
            force *= Time.fixedDeltaTime; // Scale with physics step

            collidedPlayer.rigidbody.AddForce(force);
        }
    }
}
