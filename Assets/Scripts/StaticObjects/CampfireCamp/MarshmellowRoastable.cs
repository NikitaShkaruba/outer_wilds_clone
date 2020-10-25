using Celestial;
using Common;
using PlayerLogic;
using UnityEngine;

namespace StaticObjects.CampfireCamp
{
    public class MarshmallowRoastable
    {
        private readonly Player player;
        private Campfire campfire;
        private readonly PlayerLockable playerLockable = new PlayerLockable();

        private readonly GameObject marshmallowStick;
        private float stickHorizontalRotation;
        private float stickVerticalRotation = 180f;

        public MarshmallowRoastable(Player player, GameObject marshmallowStick)
        {
            this.player = player;
            this.marshmallowStick = marshmallowStick;
        }

        public void StartCooking(Campfire campfire)
        {
            this.campfire = campfire;

            Vector3 wantedLocalPosition = 17f * (player.transform.position - campfire.transform.position).normalized;
            Quaternion wantedGlobalBodyRotation = Quaternion.LookRotation((campfire.transform.position - player.transform.position + 0.6f * campfire.transform.up).normalized);
            Quaternion wantedLocalBodyRotation = Quaternion.Inverse(campfire.transform.rotation) * Quaternion.Euler(wantedGlobalBodyRotation.eulerAngles);
            Quaternion wantedLocalCameraRotation = Quaternion.Euler(0, 0, 0);
            playerLockable.Lock(player, campfire.transform, wantedLocalPosition, wantedLocalBodyRotation, wantedLocalCameraRotation);

            marshmallowStick.SetActive(true);
        }

        public void StopCooking()
        {
            CelestialBody celestialBody = campfire.GetComponentInParent<CelestialBody>();
            playerLockable.Unlock(celestialBody.rigidbody.velocity);

            campfire = null;
            marshmallowStick.SetActive(false);
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
            RotateStick(playerInputRotation);
            AdjustStickDepth(stickExtended);
        }

        private void RotateStick(Vector2 playerInputRotation)
        {
            float verticalMouseOffset = playerInputRotation.y * GameSettings.MouseSensitivity * Time.deltaTime;
            stickVerticalRotation -= verticalMouseOffset;
            stickVerticalRotation = Mathf.Clamp(stickVerticalRotation, 140f, 190f);

            float horizontalMouseOffset = playerInputRotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
            stickHorizontalRotation += horizontalMouseOffset;
            stickHorizontalRotation = Mathf.Clamp(stickHorizontalRotation, -60f, 25f);

            marshmallowStick.transform.localRotation = Quaternion.Euler(stickVerticalRotation, stickHorizontalRotation, 0f);
        }

        private void AdjustStickDepth(bool stickExtended)
        {
            const float minDepth = 0.2f;
            const float maxDepth = 0.64f;
            const float transitionSpeed = 0.2f;

            float wantedDepth = stickExtended ? maxDepth : minDepth;
            float currentDepth = Mathf.SmoothStep(marshmallowStick.transform.localPosition.z, wantedDepth, transitionSpeed);

            marshmallowStick.transform.localPosition = new Vector3(marshmallowStick.transform.localPosition.x, marshmallowStick.transform.localPosition.y, currentDepth);
        }
    }
}
