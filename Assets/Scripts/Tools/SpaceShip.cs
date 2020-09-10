using UnityEngine;

namespace Tools
{
    public class SpaceShip : Body
    {
        [SerializeField] private GameObject hatchRotator;
        [SerializeField] private GameObject hatchGravityField;

        private bool isHatchClosed;
        private bool isHatchClosing;

        private void FixedUpdate()
        {
            if (isHatchClosing)
            {
                CloseHatchMore();
            }
        }

        public void PlayerEnteredShip()
        {
            if (isHatchClosed)
            {
                return;
            }

            isHatchClosing = true;
        }

        private void CloseHatchMore()
        {
            const float hatchClosingSpeed = -300f;
            hatchRotator.transform.Rotate(hatchClosingSpeed * Time.deltaTime, 0f, 0f);

            float angleDifference = Quaternion.Angle(hatchRotator.transform.localRotation, Quaternion.Euler(0, 0, 0));
            if (Mathf.Approximately(angleDifference, 0))
            {
                FinishClosingHatch();
            }
        }

        private void FinishClosingHatch()
        {
            isHatchClosing = false;
            isHatchClosed = true;
            hatchGravityField.SetActive(false);
        }
    }
}