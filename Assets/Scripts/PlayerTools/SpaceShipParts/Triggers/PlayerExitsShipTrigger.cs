using System;
using PlayerLogic;
using UnityEngine;

namespace PlayerTools.SpaceShipParts.Triggers
{
    /**
     * Class that helps to understand if player is exited a SpaceShip
     */
    public class PlayerExitsShipTrigger : MonoBehaviour
    {
        public event Action OnPlayerExitedShip;

        private void OnTriggerExit(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }

            OnPlayerExitedShip?.Invoke();
        }
    }
}
