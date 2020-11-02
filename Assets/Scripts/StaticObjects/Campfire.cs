using PlayerLogic;
using UnityEngine;

namespace StaticObjects
{
    /**
     * Class that represents Campfire that can damage the player
     */
    [RequireComponent(typeof(Collider))]
    public class Campfire : MonoBehaviour
    {
        [SerializeField] private float fireDamage;

        private void OnTriggerStay(Collider other)
        {
            Player collidedPlayer = other.GetComponent<Player>();
            if (collidedPlayer == null)
            {
                return;
            }

            collidedPlayer.Damageable.Damage(fireDamage);
        }
    }
}
