using PlayerTools.SpaceShipParts;
using Universe;

namespace PlayerTools
{
    public class SpaceShip : SpaceBody
    {
        // External components
        public SpaceShipThrusters thrusters;
        public SpaceShipAccelerationShowcase accelerationShowcase;
        public SpaceShipFlashlight flashlight;

        private new void Awake()
        {
            base.Awake();
        }

        private void FixedUpdate()
        {
            ApplyGravity();
        }
    }
}
