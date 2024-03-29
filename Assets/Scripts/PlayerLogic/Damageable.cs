using System;
using UnityEngine;

namespace PlayerLogic
{
    /**
     * Class that handles Health Points logic
     */
    public class Damageable
    {
        private float healthPoints;

        private readonly float maxHealthPoints;
        private readonly float minHealthPoints;

        public bool HasFullHealthPoints => Mathf.Approximately(healthPoints, maxHealthPoints);
        private bool HasNoHealthPoints => Mathf.Approximately(healthPoints, minHealthPoints);

        public event Action<float> OnHealthPointsChange;
        public event Action OnTakingDamage;
        public event Action OnNoHealthPointsRemaining;

        public Damageable(float maxHealthPoints)
        {
            this.maxHealthPoints = maxHealthPoints;
            minHealthPoints = 0f;

            UpdateHealthPoints(this.maxHealthPoints);
        }

        public void Damage(float newHealthPoints)
        {
            UpdateHealthPoints(healthPoints - newHealthPoints);
            OnTakingDamage?.Invoke();
        }

        public void Heal(float newHealthPoints)
        {
            UpdateHealthPoints(healthPoints + newHealthPoints);
        }

        private void UpdateHealthPoints(float newHealthPoints)
        {
            healthPoints = Mathf.Clamp(newHealthPoints, minHealthPoints, maxHealthPoints);

            OnHealthPointsChange?.Invoke(newHealthPoints);

            if (HasNoHealthPoints)
            {
                OnNoHealthPointsRemaining?.Invoke();
            }
        }
    }
}
