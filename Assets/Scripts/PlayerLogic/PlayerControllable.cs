using System;
using UnityEngine;

namespace PlayerLogic
{
    // I don't want to use Unity's new input system there, because the old system is more than fine for pc-only build
    public class PlayerControllable : MonoBehaviour
    {
        [Header("Movement")]
        public Vector3 movement;
        public bool jump;

        [Header("Rotation")]
        public Vector2 rotation;
        public bool alternativeRotate;

        [Header("Other")]
        public bool extendMarshmallowStick;

        [Header("Dev")]
        public bool toggleCornerDebug;
        public bool toggleCursorLock;
        public event Action OnCornerDebugToggle;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            ProcessMovementInput();
            ProcessRotationInput();
            ProcessOtherInput();
            ProcessDebugInput();
        }

        private void ProcessOtherInput()
        {
            extendMarshmallowStick = Input.GetKey(KeyCode.F);
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

        /**
         * I don't use Input.GetKeyDown, because I want those keys to be present in the inspector
         */
        private void ProcessDebugInput()
        {
            bool previousToggleCornerDebug = toggleCornerDebug;
            toggleCornerDebug = Input.GetKey(KeyCode.F1);
            if (!previousToggleCornerDebug && toggleCornerDebug)
            {
                OnCornerDebugToggle?.Invoke();
            }

            bool previousToggleCursorLock = toggleCursorLock;
            toggleCursorLock = Input.GetKey(KeyCode.F2);
            if (!previousToggleCursorLock && toggleCursorLock)
            {
                // Let's handle it right there. I have no need for a class right now
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}
