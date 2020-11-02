using PlayerLogic;
using UnityEngine;

namespace StaticObjects
{
    /**
     * Class that represents tree - that can refresh player's SpaceSuit oxygen tank
     */
    [RequireComponent(typeof(Collider))]
    public class Tree : MonoBehaviour
    {
        private const float OxygenRefillSpeed = 0.5f;

        private void OnTriggerStay(Collider other)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            collidedPlayer.spaceSuit.FillOxygenTank(OxygenRefillSpeed);
        }
    }
}
