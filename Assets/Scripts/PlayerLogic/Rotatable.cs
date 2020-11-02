using Common;
using UnityEngine;

namespace PlayerLogic
{
    public class Rotatable
    {
        private float headVerticalRotation;

        public void Rotate(Transform transform, Camera camera, PlayerControllable playerControllable)
        {
            ProcessVerticalInput(camera, playerControllable);
            ProcessHorizontalInput(transform, playerControllable);
        }

        private void ProcessVerticalInput(Camera camera, PlayerControllable playerControllable)
        {
            float verticalMouseOffset = playerControllable.rotation.y * GameSettings.MouseSensitivity * Time.deltaTime;

            headVerticalRotation -= verticalMouseOffset;
            headVerticalRotation = Mathf.Clamp(headVerticalRotation, -90f, 90f); // We don't want our player to roll over with the camera :)

            camera.transform.localRotation = Quaternion.Euler(headVerticalRotation, 0f, 0f);
        }

        private static void ProcessHorizontalInput(Transform transform, PlayerControllable playerControllable)
        {
            float horizontalMouseOffset = playerControllable.rotation.x * GameSettings.MouseSensitivity * Time.deltaTime;

            if (playerControllable.alternativeRotate)
            {
                transform.Rotate(Vector3.forward * -horizontalMouseOffset);
            }
            else
            {
                transform.Rotate(Vector3.up * horizontalMouseOffset);
            }
        }
    }
}
