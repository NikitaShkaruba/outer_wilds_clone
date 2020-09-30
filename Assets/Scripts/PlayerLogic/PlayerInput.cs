using System;
using UnityEngine;

namespace PlayerLogic
{
    // I don't want to use Unity's new input system there, because the old system is more than fine for pc-only build
    public class PlayerInput : MonoBehaviour
    {
        [Header("Movement")]
        public Vector3 movement;
        public bool jump;

        [Header("Rotation")]
        public Vector2 rotation;
        public bool alternativeRotate;

        [Header("Dev")]
        public bool toggleCornerDebug;
        public bool toggleCursorLock;
        public event Action onCornerDebugToggle;

        private void Update()
        {
            ProcessMovementInput();
            ProcessRotationInput();
            ProcessDebugInput();
        }

        private void ProcessMovementInput()
        {
            // Move
            movement.x = CalculateDirection(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S));
            movement.z = CalculateDirection(Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.A));
            movement.y = CalculateDirection(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));

            // Jump
            jump = Input.GetKey(KeyCode.Space);
        }

        private static float CalculateDirection(bool oneDirectionKeyPressed, bool otherDirectionKeyPressed)
        {
            if (oneDirectionKeyPressed)
            {
                return 1f;
            }

            if (otherDirectionKeyPressed)
            {
                return -1f;
            }

            return 0f;
        }

        private void ProcessRotationInput()
        {
            rotation.x = Input.GetAxis("Mouse X");
            rotation.y = Input.GetAxis("Mouse Y");

            alternativeRotate = Input.GetKey(KeyCode.R);
        }

        private void ProcessDebugInput()
        {
            toggleCornerDebug = Input.GetKeyDown(KeyCode.F1);
            if (toggleCornerDebug)
            {
                onCornerDebugToggle?.Invoke();
            }

            toggleCursorLock = Input.GetKeyDown(KeyCode.F2);
            if (toggleCursorLock)
            {
                // Let's handle it right there. I have no need for a class right now
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}
