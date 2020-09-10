﻿using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class PlayerEnteredShipTrigger : MonoBehaviour
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
