using Common;
using UnityEngine;

namespace PlayerLogic
{
    public class Rotatable
    {
        private float headVerticalRotation;

        public void Rotate(Transform transform, Camera camera, PlayerInput playerInput)
        {
            float horizontalMouseOffset = playerInput.rotation.x * GameSettings.MouseSensitivity * Time.deltaTime;
            float verticalMouseOffset = playerInput.rotation.y * GameSettings.MouseSensitivity * Time.deltaTime;

            headVerticalRotation -= verticalMouseOffset;
            headVerticalRotation = Mathf.Clamp(headVerticalRotation, -90f, 90f); // We don't want our player to roll over with the camera :)
            camera.transform.localRotation = Quaternion.Euler(headVerticalRotation, 0f, 0f);

            if (playerInput.alternativeRotate)
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
