using System;
using UnityEngine;

namespace PlayerTools.SpaceSuitParts
{
    /**
     * Class that can store some percentage of some gas (or literally anything) inside
     */
    public class SpaceSuitTank
    {
        private const float MaxFillPercentage = 100f;
        private const float MinFillPercentage = 0f;

        private float filledPercentage = MaxFillPercentage;

        public bool IsFull => Mathf.Approximately(filledPercentage, MaxFillPercentage);
        public bool IsEmpty => Mathf.Approximately(filledPercentage, MinFillPercentage);

        // Events
        public event Action<float> OnFillPercentageChanged;

        public void Deplete(float percentage)
        {
            UpdateFilledPercentage(filledPercentage - percentage);
        }

        public void Fill(float percentage)
        {
            UpdateFilledPercentage(filledPercentage + percentage);
        }

        private void UpdateFilledPercentage(float newFilledPercentage)
        {
            filledPercentage = Mathf.Clamp(newFilledPercentage, MinFillPercentage, MaxFillPercentage);
            OnFillPercentageChanged?.Invoke(filledPercentage);
        }
    }
}
