using UnityEngine;

namespace PlayerLogic
{
    public class Jumpable
    {
        private const float JumpPowerAccumulationSpeed = 50f;
        private const float MaxJumpPower = 400f;

        private float accumulatedJumpPower;

        public bool ReadyToJump => !Mathf.Approximately(accumulatedJumpPower, 0f);
        public bool AccumulatedMaxJumpPower => Mathf.Approximately(accumulatedJumpPower, MaxJumpPower);

        public void AccumulateJumpPower()
        {
            if (accumulatedJumpPower >= MaxJumpPower)
            {
                return;
            }

            // Accumulate jump power
            accumulatedJumpPower += JumpPowerAccumulationSpeed;
        }

        public float Jump()
        {
            float jumpPower = accumulatedJumpPower;

            accumulatedJumpPower = 0f;

            return jumpPower;
        }
    }
}
