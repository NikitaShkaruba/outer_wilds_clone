using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class PlayerExitsShipTrigger : MonoBehaviour
    {
        [SerializeField] private SpaceShip spaceShip;
    
        private void OnTriggerExit(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }
        
            spaceShip.PlayerExitedShip();
        }
    }
}
