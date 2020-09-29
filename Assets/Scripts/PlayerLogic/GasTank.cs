using UnityEngine;

namespace PlayerLogic
{
    public class GasTank
    {
        private const float MaxTankPercentage = 100f;
        private const float MinTankPercentage = 0f;

        public float FilledPercentage = MaxTankPercentage;

        public bool IsFull => Mathf.Approximately(FilledPercentage, MaxTankPercentage);
        public bool IsEmpty => Mathf.Approximately(FilledPercentage, MinTankPercentage);

        public void Waste(float percentage)
        {
            UpdateFilledPercentage(FilledPercentage - percentage);
        }

        public void Refuel(float percentage)
        {
            UpdateFilledPercentage(FilledPercentage + percentage);
        }

        private void UpdateFilledPercentage(float newFuelPercentage)
        {
            FilledPercentage = Mathf.Clamp(newFuelPercentage, MinTankPercentage, MaxTankPercentage);
        }
    }
}
