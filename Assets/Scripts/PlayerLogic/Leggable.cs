using UnityEngine;

namespace PlayerLogic
{
    public class Leggable
    {
        private const float MoveSpeed = 7f;
        private const float DistanceFromBodyCenterToGround = 1.1f;

        private readonly Player player;
        private readonly LayerMask groundCheckLayerMask;

        public Leggable(Player player)
        {
            this.player = player;
            groundCheckLayerMask = LayerMask.GetMask("Planets", "Objects");
        }

        public static float Run()
        {
            return MoveSpeed;
        }

        public bool IsGrounded()
        {
            Transform cachedPlayerTransform = player.transform;
            return Physics.Raycast(cachedPlayerTransform.position, -cachedPlayerTransform.up, DistanceFromBodyCenterToGround, groundCheckLayerMask);
        }
    }
}
