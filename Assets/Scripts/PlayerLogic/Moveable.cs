using UI.Debug;
using UnityEngine;

namespace PlayerLogic
{
    /**
     * Class that handles player moving logic
     */
    public class Moveable
    {
        private readonly Player player;
        private readonly Groundable groundable;
        private readonly Jumpable jumpable;

        private const float LegsMoveSpeed = 7f;

        public Moveable(Player player)
        {
            this.player = player;
            groundable = new Groundable(player);
            jumpable = new Jumpable(player);
        }

        public void Move(PlayerControllable playerControllable)
        {
            Vector3 playerVerticalMotion = player.transform.up * playerControllable.movement.y;
            Vector3 playerHorizontalMotion = player.transform.forward * playerControllable.movement.x +
                                             player.transform.right * playerControllable.movement.z;

            if (groundable.IsGrounded())
            {
                WalkByFoot(playerHorizontalMotion);
                HandleJumpLogic(playerControllable);
            }
            else
            {
                FireHorizontalThrusters(playerHorizontalMotion);
            }

            if (DoPlayerWantsToFlyVertically(playerControllable))
            {
                FireVerticalThrusters(playerControllable, playerVerticalMotion);
            }

            CornerDebug.AddDebug("IsOnTheGround = " + groundable.IsGrounded());
        }

        private void WalkByFoot(Vector3 playerHorizontalMotion)
        {
            // Movement by foot with AddForce is buggy, so for now MovePosition will work.
            // 03 November 2020 Update: Should've used AddForce :D

            Vector3 playerPositionAddition = playerHorizontalMotion;
            playerPositionAddition *= LegsMoveSpeed;
            playerPositionAddition *= Time.deltaTime;

            player.rigidbody.MovePosition(player.rigidbody.position + playerPositionAddition);
        }

        private void FireHorizontalThrusters(Vector3 playerHorizontalMotion)
        {
            Vector3 horizontalThrustersForce = playerHorizontalMotion;
            horizontalThrustersForce *= player.spaceSuit.FireHorizontalThrusters();
            horizontalThrustersForce *= Time.deltaTime;

            player.rigidbody.AddForce(horizontalThrustersForce);
        }

        private void FireVerticalThrusters(PlayerControllable playerControllable, Vector3 playerVerticalMotion)
        {
            bool useSuperFuel = playerControllable.movement.y > 0f && playerControllable.jump;

            Vector3 verticalThrustersForce = playerVerticalMotion;
            verticalThrustersForce *= player.spaceSuit.FireVerticalThrusters(useSuperFuel);
            verticalThrustersForce *= Time.deltaTime;

            player.rigidbody.AddForce(verticalThrustersForce);
        }

        private void HandleJumpLogic(PlayerControllable playerControllable)
        {
            if (playerControllable.jump)
            {
                jumpable.AccumulateJumpPower();
            }
            else if (!playerControllable.jump && jumpable.ReadyToJump)
            {
                Vector3 jumpMotion = player.transform.up;
                jumpMotion *= jumpable.Jump();
                player.rigidbody.AddForce(jumpMotion); // There's no Time.deltaTime, because it's a single force push
            }
        }

        private static bool DoPlayerWantsToFlyVertically(PlayerControllable playerControllable)
        {
            return !Mathf.Approximately(playerControllable.movement.y, 0f);
        }
    }
}
