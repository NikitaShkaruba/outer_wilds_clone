using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class SpaceShipHatch : MonoBehaviour
    {
        [SerializeField] private SpaceShip spaceShip;

        public bool IsClosed => spaceShip.isHatchClosed;

        public void Open()
        {
            spaceShip.OpenHatch();
        }
    }
}