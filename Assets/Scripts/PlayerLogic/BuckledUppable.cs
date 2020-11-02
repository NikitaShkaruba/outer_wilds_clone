using PlayerTools.SpaceShipParts;
using UnityEngine;

namespace PlayerLogic
{
    /**
     * Class that handles buckling up into a SpaceShitSeat logic, that gives buckled up player some methods to access SpaceShip's interface.
     * Todo: move buckling-up logic here from SpaceShipSeat
     */
    public class BuckledUppable
    {
        private SpaceShipSeat buckledUpSpaceShipSeat;
        private readonly Player playerOwner;

        public BuckledUppable(Player playerOwner)
        {
            this.playerOwner = playerOwner;
        }

        public void BuckleUpIntoSpaceShipSeat(SpaceShipSeat spaceShipSeat)
        {
            buckledUpSpaceShipSeat = spaceShipSeat;
            buckledUpSpaceShipSeat.StartBucklingUp(playerOwner);
        }

        public void UnbuckleFromSpaceShipSeat()
        {
            buckledUpSpaceShipSeat.Unbuckle();
            buckledUpSpaceShipSeat = null;
        }

        public bool IsBuckledUp()
        {
            return buckledUpSpaceShipSeat != null;
        }

        public void PilotShip(Vector3 wantedAcceleration, Vector2 wantedRotation, bool alternativeRotationEnabled)
        {
            buckledUpSpaceShipSeat.spaceShipInterface.PilotShip(wantedAcceleration, wantedRotation, alternativeRotationEnabled);
        }

        public void ToggleFlashlight()
        {
            buckledUpSpaceShipSeat.spaceShipInterface.ToggleFlashlight();
        }
    }
}
