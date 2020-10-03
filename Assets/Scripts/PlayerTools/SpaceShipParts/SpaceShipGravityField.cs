using Cinemachine.Utility;
using PlayerTools.SpaceShipParts.Triggers;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipGravityField : MonoBehaviour
    {
        [SerializeField] private PlayerExitsShipTrigger playerExitsShipTrigger;
        [SerializeField] private GameObject visualEffect;

        public float pullPower;

        public bool IsActive => visualEffect.activeSelf;

        public void Awake()
        {
            playerExitsShipTrigger.OnPlayerExitedShip += Enable;
        }

        private void OnDestroy()
        {
            playerExitsShipTrigger.OnPlayerExitedShip -= Enable;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsActive)
            {
                return;
            }

            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            PullPlayer(collidedPlayer);
        }

        private void PullPlayer(Player player)
        {
            Transform cachedTransform = transform;
            Vector3 cachedTransformUp = cachedTransform.up;

            Vector3 positionDifference = cachedTransform.position - player.Position;
            Vector3 directionToCenter = positionDifference.ProjectOntoPlane(cachedTransformUp);

            Vector3 force = directionToCenter; // Pull towards the center
            force += cachedTransformUp; // Pull up
            force *= pullPower; // Add power
            force *= Time.fixedDeltaTime; // Scale with physics step

            player.rigidbody.AddForce(force);
        }

        public void Disable()
        {
            visualEffect.SetActive(false);
        }

        private void Enable()
        {
            visualEffect.SetActive(true);
        }
    }
}
