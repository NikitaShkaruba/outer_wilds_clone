using UI.Debug;
using UnityEngine;

namespace PlayerLogic
{
    public class Moveable
    {
        private readonly Player player;
        private readonly Leggable leggable;
        private readonly Jumpable jumpable;

        public Moveable(Player player)
        {
            this.player = player;
            leggable = new Leggable(player);
            jumpable = new Jumpable(player);
        }

        public void Move(PlayerControllable playerControllable)
        {
            Vector3 playerVerticalMotion = player.transform.up * playerControllable.movement.y;
            Vector3 playerHorizontalMotion = player.transform.forward * playerControllable.movement.x +
                                             player.transform.right * playerControllable.movement.z;

            if (leggable.IsGrounded())
            {
                WalkByFoot(playerHorizontalMotion);
            }
            else
            {
                FireHorizontalThrusters(playerHorizontalMotion);
            }

            if (DoPlayerWantsToFlyVertically(playerControllable))
            {
                FireVerticalThrusters(playerControllable, playerVerticalMotion);
            }

            if (leggable.IsGrounded())
            {
                HandleJumpLogic(playerControllable);
            }

            CornerDebug.AddDebug("IsOnTheGround = " + leggable.IsGrounded());
        }

        private void WalkByFoot(Vector3 playerHorizontalMotion)
        {
            // Movement by foot with AddForce is buggy, so for now MovePosition will work.
            // 03 November 2020 Update: Should've used AddForce :D

            Vector3 playerPositionAddition = playerHorizontalMotion;
            playerPositionAddition *= Leggable.Run();
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
