using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipInterface : MonoBehaviour
    {
        [SerializeField] private SpaceShip spaceShip;

        public void PilotShip(Vector3 wantedAcceleration, Vector2 wantedRotation, bool alternativeRotationEnabled)
        {
            spaceShip.thrusters.UpdateInput(wantedAcceleration, wantedRotation, alternativeRotationEnabled);
            spaceShip.accelerationShowcase.UpdateInput(wantedAcceleration);
        }

        public void ToggleFlashlight()
        {
            spaceShip.flashlight.Toggle();
        }
    }
}
