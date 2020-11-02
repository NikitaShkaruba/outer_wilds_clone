using System;
using PlayerTools;

namespace PlayerLogic
{
    public class OxygenBreathable
    {
        private bool hasSomethingToBreathe = true;
        public event Action OnNoOxygenLeft;

        public void BreatheOxygen(SpaceSuit spaceSuit)
        {
            hasSomethingToBreathe = spaceSuit.GiveOxygenToBreathe();

            if (!hasSomethingToBreathe)
            {
                // Maybe add a little delay later (player can survive without oxygen for 30 seconds or so)
                OnNoOxygenLeft?.Invoke();
            }
        }
    }
}
