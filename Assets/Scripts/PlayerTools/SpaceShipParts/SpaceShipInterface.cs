using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    /**
     * Class that provides ways for player to interact with a SpaceShip
     */
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
