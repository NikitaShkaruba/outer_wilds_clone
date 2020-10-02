using System;
using UnityEngine;

namespace PlayerTools.SpaceShipParts.Triggers
{
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
