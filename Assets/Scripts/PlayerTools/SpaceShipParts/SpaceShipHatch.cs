using PlayerTools.SpaceShipParts.Triggers;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipHatch : MonoBehaviour
    {
        [SerializeField] private GameObject rotator;
        [SerializeField] private SpaceShipGravityField gravityField;
        [SerializeField] private PlayerEntersShipTrigger playerEntersShipTrigger;

        public bool isClosed;
        private bool itMoving;

        public void Awake()
        {
            playerEntersShipTrigger.OnPlayerEnteredShip += CloseOnPlayerExit;
        }

        public void FixedUpdate()
        {
            if (itMoving)
            {
                MoveHatch();
            }
        }

        public void Toggle()
        {
            itMoving = true;
        }

        private void CloseOnPlayerExit()
        {
            if (isClosed || !gravityField.IsActive)
            {
                return;
            }

            Toggle();
        }

        private void MoveHatch()
        {
            float hatchClosingSpeed = 300f;
            hatchClosingSpeed *= isClosed ? 1 : -1;

            rotator.transform.Rotate(hatchClosingSpeed * Time.deltaTime, 0f, 0f);

            float finalXEuler = isClosed ? -90 : 90;
            Quaternion desiredRotation = Quaternion.Euler(finalXEuler, 0, 0);

            float angleDifference = Quaternion.Angle(rotator.transform.localRotation, desiredRotation);
            if (Mathf.Approximately(angleDifference, 0))
            {
                FinishHatchMoving();
            }
        }

        private void FinishHatchMoving()
        {
            itMoving = false;
            isClosed = !isClosed;

            if (isClosed)
            {
                gravityField.Disable();
            }
        }
    }
}
