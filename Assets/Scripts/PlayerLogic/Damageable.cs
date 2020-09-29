using UnityEngine;

namespace PlayerLogic
{
    public class Damageable
    {
        public float HealthPoints;

        private readonly float maxHealthPoints;
        private readonly float minHealthPoints;

        public bool HasFullHealthPoints => Mathf.Approximately(HealthPoints, maxHealthPoints);
        public bool HasNoHealthPoints => Mathf.Approximately(HealthPoints, minHealthPoints);

        public Damageable(float maxHealthPoints)
        {
            this.maxHealthPoints = maxHealthPoints;
            minHealthPoints = 0f;

            UpdateHealthPoints(this.maxHealthPoints);
        }

        public void Damage(float healthPoints)
        {
            UpdateHealthPoints(HealthPoints - healthPoints);
        }

        public void Heal(float healthPoints)
        {
            UpdateHealthPoints(HealthPoints + healthPoints);
        }

        private void UpdateHealthPoints(float healthPoints)
        {
            HealthPoints = Mathf.Clamp(healthPoints, minHealthPoints, maxHealthPoints);
        }
    }
}
