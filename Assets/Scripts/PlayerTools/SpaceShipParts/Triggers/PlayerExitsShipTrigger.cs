using System;
using UnityEngine;

namespace PlayerTools.SpaceShipParts.Triggers
{
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
