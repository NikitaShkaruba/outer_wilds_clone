using System;

namespace PlayerLogic
{
    public class Dieable
    {
        public bool IsDead;
        public event Action OnDeath;

        public void Die()
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}
