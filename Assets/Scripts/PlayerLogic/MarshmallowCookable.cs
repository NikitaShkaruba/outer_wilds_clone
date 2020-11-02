using Celestial;
using Common;
using PlayerLogic.MarshmallowCookableParts;
using StaticObjects;
using UnityEngine;

namespace PlayerLogic
{
    [RequireComponent(typeof(Player))]
    public class MarshmallowCookable : MonoBehaviour
    {
        private Player player;
        private Campfire campfire;
        public GameObject marshmallowStick;
        [HideInInspector] public Marshmallow marshmallow;

        private readonly PlayerLockable playerLockable = new PlayerLockable();
        [SerializeField] private GameObject marshmallowPrefab;

        private float stickHorizontalRotation;
        private float stickVerticalRotation = 180f;

        public void Start()
        {
            player = GetComponent<Player>();
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

            if (marshmallow)
            {
                Destroy(marshmallow.gameObject);
            }

            ReplaceMarshmallow();
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

        public void HoldMarshmallowStick(Vector2 playerInputRotation, bool stickExtended)
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
            stickVerticalRotation += verticalMouseOffset;
            stickVerticalRotation = Mathf.Clamp(stickVerticalRotation, 170, 205f);

            float horizontalMouseOffset = playerInputRotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
            stickHorizontalRotation += horizontalMouseOffset;
            stickHorizontalRotation = Mathf.Clamp(stickHorizontalRotation, -220f, -160f);

            marshmallowStick.transform.localRotation = Quaternion.Euler(stickVerticalRotation, stickHorizontalRotation, 0f);
        }

        private void AdjustStickDepth(bool stickExtended)
        {
            const float minDepth = -0.4f;
            const float maxDepth = 0.2f;
            const float transitionSpeed = 0.2f;

            float wantedDepth = stickExtended ? maxDepth : minDepth;
            float currentDepth = Mathf.SmoothStep(marshmallowStick.transform.localPosition.z, wantedDepth, transitionSpeed);

            marshmallowStick.transform.localPosition = new Vector3(marshmallowStick.transform.localPosition.x, marshmallowStick.transform.localPosition.y, currentDepth);
        }

        public void ThrowBurnedMarshmallow()
        {
            marshmallow.rigidbody.velocity = ComputeThrownMarshmallowVelocity();
            marshmallow.rigidbody.isKinematic = false;
            marshmallow.collider.isTrigger = false;
            marshmallow.transform.SetParent(null);
            marshmallow.InitiateSelfDestruction();

            marshmallow = null;
        }

        private Vector3 ComputeThrownMarshmallowVelocity()
        {
            CelestialBody parentCelestialBodyWithVelocity = player.GetComponentInParent<CelestialBody>();
            Vector3 pushVelocity = marshmallow.transform.forward * 5f + marshmallow.transform.up * 2f;

            return parentCelestialBodyWithVelocity.rigidbody.velocity + pushVelocity;
        }

        public void EatMarshmallow()
        {
            Destroy(marshmallow.gameObject);
        }

        public void ReplaceMarshmallow()
        {
            GameObject marshmallowStickMarshmallow = Instantiate(marshmallowPrefab, marshmallowStick.transform);
            marshmallowStickMarshmallow.transform.localPosition = new Vector3(0f, 0f, 1f);
            marshmallowStickMarshmallow.transform.localRotation = Quaternion.identity;

            marshmallow = marshmallowStickMarshmallow.GetComponent<Marshmallow>();
        }
    }
}
