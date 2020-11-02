using System;
using PlayerLogic;
using UnityEngine;

namespace PlayerTools.SpaceShipParts.Triggers
{
    /**
     * Class that helps to understand if player is entered a SpaceShip
     */
    public class PlayerEntersShipTrigger : MonoBehaviour
    {
        public event Action OnPlayerEnteredShip;

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }

            OnPlayerEnteredShip?.Invoke();
        }
    }
}
