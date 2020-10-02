using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipInterface : MonoBehaviour
    {
        [SerializeField] private SpaceShip spaceShip;

        public void PilotShip(Vector3 playerInputMovement, Vector2 playerInputRotation, bool playerInputAlternativeRotate)
        {
            spaceShip.Pilot(playerInputMovement, playerInputRotation, playerInputAlternativeRotate);
        }

        public void ToggleFlashlight()
        {
            spaceShip.ToggleFlashlight();
        }
    }
}
