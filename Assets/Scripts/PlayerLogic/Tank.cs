using UnityEngine;

namespace PlayerLogic
{
    public class Tank
    {
        private const float MaxFillPercentage = 100f;
        private const float MinFillPercentage = 0f;

        public float FilledPercentage = MaxFillPercentage;

        public bool IsFull => Mathf.Approximately(FilledPercentage, MaxFillPercentage);
        public bool IsEmpty => Mathf.Approximately(FilledPercentage, MinFillPercentage);

        public void Deplete(float percentage)
        {
            UpdateFilledPercentage(FilledPercentage - percentage);
        }

        public void Fill(float percentage)
        {
            UpdateFilledPercentage(FilledPercentage + percentage);
        }

        private void UpdateFilledPercentage(float newFilledPercentage)
        {
            FilledPercentage = Mathf.Clamp(newFilledPercentage, MinFillPercentage, MaxFillPercentage);
        }
    }
}
