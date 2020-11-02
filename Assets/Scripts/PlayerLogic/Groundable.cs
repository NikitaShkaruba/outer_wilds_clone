using UnityEngine;

namespace PlayerLogic
{
    /**
     * Class that handles grounding logic
     */
    public class Groundable
    {
        private const float DistanceFromBodyCenterToGround = 1.1f;

        private readonly Player player;
        private readonly LayerMask groundCheckLayerMask;

        public Groundable(Player player)
        {
            this.player = player;
            groundCheckLayerMask = LayerMask.GetMask("Planets", "Objects");
        }

        public bool IsGrounded()
        {
            Transform cachedPlayerTransform = player.transform;
            return UnityEngine.Physics.Raycast(cachedPlayerTransform.position, -cachedPlayerTransform.up, DistanceFromBodyCenterToGround, groundCheckLayerMask);
        }
    }
}
