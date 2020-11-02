using PlayerLogic;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    /**
     * Class that handles logic for SpaceShip's ResourcesStation which can heal player and restore his fuel
     */
    public class SpaceShipHealthAndFuelStation : MonoBehaviour
    {
        private const float FuelRefillSpeed = 2f;
        private const float HealthRefillSpeed = 7f;

        private Player connectedPlayer;

        private void FixedUpdate()
        {
            if (connectedPlayer == null)
            {
                return;
            }

            RefillHealthAndFuel();
        }

        public void ConnectPlayer(Player player)
        {
            connectedPlayer = player;
        }

        private void RefillHealthAndFuel()
        {
            connectedPlayer.Damageable.Heal(HealthRefillSpeed);
            connectedPlayer.spaceSuit.FillFuelTank(FuelRefillSpeed);

            if (!CanUseRefill(connectedPlayer))
            {
                connectedPlayer = null;
            }
        }

        public static bool CanUseRefill(Player player)
        {
            return !player.Damageable.HasFullHealthPoints || !player.spaceSuit.IsFuelTankFull;
        }
    }
}
