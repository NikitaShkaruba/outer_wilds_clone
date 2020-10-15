using UnityEngine;

namespace PlayerLogic
{
    public class Jumpable
    {
        // Jumping
        private const float JumpPowerAccumulationSpeed = 50f;
        private const float MaxJumpPower = 1000f;
        private float accumulatedJumpPower;

        // Shrinking on accumulation
        private readonly Player player;
        private readonly Vector3 scaleBeforeJumpShrink;
        private const float PlayerShrinkSpeed = 0.05f;

        public bool ReadyToJump => !Mathf.Approximately(accumulatedJumpPower, 0f);

        public Jumpable(Player player)
        {
            this.player = player;
            scaleBeforeJumpShrink = this.player.transform.localScale;
        }

        public void AccumulateJumpPower()
        {
            if (accumulatedJumpPower >= MaxJumpPower)
            {
                return;
            }

            accumulatedJumpPower += JumpPowerAccumulationSpeed;

            ShrinkPlayer();
        }

        public float Jump()
        {
            float jumpPower = accumulatedJumpPower;
            accumulatedJumpPower = 0f;

            ResetPlayerShrinkScale();

            return MaxJumpPower;
            return jumpPower;
        }

        private void ShrinkPlayer()
        {
            player.transform.localScale -= new Vector3(0f, PlayerShrinkSpeed, 0f);
        }

        private void ResetPlayerShrinkScale()
        {
            player.transform.localScale = scaleBeforeJumpShrink;
        }
    }
}
