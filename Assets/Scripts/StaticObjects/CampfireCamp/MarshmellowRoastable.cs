using Celestial;
using PlayerLogic;
using UnityEngine;

namespace StaticObjects.CampfireCamp
{
    public class MarshmallowRoastable
    {
        private readonly Player player;
        private Campfire campfire;
        private readonly PlayerLockable playerLockable = new PlayerLockable();

        public MarshmallowRoastable(Player player)
        {
            this.player = player;
        }

        public void StartCooking(Campfire campfire)
        {
            this.campfire = campfire;

            Vector3 wantedLocalPosition = 17f * (player.transform.position - campfire.transform.position).normalized;
            Quaternion wantedGlobalBodyRotation = Quaternion.LookRotation((campfire.transform.position - player.transform.position + 0.6f * campfire.transform.up).normalized);
            Quaternion wantedLocalBodyRotation = Quaternion.Inverse(campfire.transform.rotation) * Quaternion.Euler(wantedGlobalBodyRotation.eulerAngles);
            Quaternion wantedLocalCameraRotation = Quaternion.Euler(0, 0, 0);
            playerLockable.Lock(player, campfire.transform, wantedLocalPosition, wantedLocalBodyRotation, wantedLocalCameraRotation);
        }

        public void StopCooking()
        {
            CelestialBody celestialBody = campfire.GetComponentInParent<CelestialBody>();
            playerLockable.Unlock(celestialBody.rigidbody.velocity);
            campfire = null;
        }

        public bool IsCooking()
        {
            return campfire != null;
        }

        public void Cook(Vector2 playerInputRotation, bool stickExtended)
        {
            if (playerLockable.IsLocked)
            {
                playerLockable.Process();
            }

            MoveMarshmallowStick(playerInputRotation, stickExtended);
        }

        private void MoveMarshmallowStick(Vector2 playerInputRotation, bool stickExtended)
        {
            Debug.Log("Rotation: " + playerInputRotation);
            Debug.Log("Stick extended: " + stickExtended);
        }
    }
}
