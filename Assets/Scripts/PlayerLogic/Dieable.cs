using System;

namespace PlayerLogic
{
    /**
     * Class that handles dying logic
     */
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
