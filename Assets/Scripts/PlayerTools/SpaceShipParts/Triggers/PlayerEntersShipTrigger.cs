using UnityEngine;

namespace PlayerTools.SpaceShipParts.Triggers
{
    public class PlayerEntersShipTrigger : MonoBehaviour
    {
        [SerializeField] private SpaceShip spaceShip;

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }

            spaceShip.PlayerEnteredShip();
        }
    }
}
