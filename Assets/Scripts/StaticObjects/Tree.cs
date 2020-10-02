using UnityEngine;

namespace StaticObjects
{
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

            collidedPlayer.SpaceSuit.FillOxygenTank(OxygenRefillSpeed);
        }
    }
}
