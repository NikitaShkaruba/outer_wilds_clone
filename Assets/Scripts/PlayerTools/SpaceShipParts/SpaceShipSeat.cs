using Common;
using PlayerLogic;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    /**
     * Class that helps to buckle up the player, that provides spaceShipInterface link for him also
     */
    public class SpaceShipSeat : MonoBehaviour
    {
        public SpaceShip spaceShip;
        public SpaceShipInterface spaceShipInterface;
        public Player seatedPlayer;
        private readonly PlayerLockable playerLockable = new PlayerLockable();

        public void FixedUpdate()
        {
            if (seatedPlayer == null)
            {
                return;
            }

            if (playerLockable.IsLocked)
            {
                playerLockable.Process();
            }
        }

        public void StartBucklingUp(Player player)
        {
            seatedPlayer = player;

            Vector3 wantedLocalPosition = new Vector3(0, 0.5f, 1.1f);
            Quaternion wantedBodyRotation = Quaternion.Euler(BlenderBugFixes.TransformBlenderEulerAngles(new Vector3(0, 0, 0)));
            Quaternion wantedCameraRotation = Quaternion.Euler(0, 0, 0);
            playerLockable.Lock(player, transform, wantedLocalPosition, wantedBodyRotation, wantedCameraRotation);
        }

        public void Unbuckle()
        {
            playerLockable.Unlock(spaceShip.rigidbody.velocity);
            seatedPlayer = null;
        }
    }
}
