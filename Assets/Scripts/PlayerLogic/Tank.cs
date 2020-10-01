using System;
using UnityEngine;

namespace PlayerLogic
{
    public class Tank
    {
        private const float MaxFillPercentage = 100f;
        private const float MinFillPercentage = 0f;

        private float filledPercentage = MaxFillPercentage;

        public bool IsFull => Mathf.Approximately(filledPercentage, MaxFillPercentage);
        public bool IsEmpty => Mathf.Approximately(filledPercentage, MinFillPercentage);

        // Events
        public event Action<float> FillPercentageChanged;

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
            FillPercentageChanged?.Invoke(filledPercentage);
        }
    }
}
