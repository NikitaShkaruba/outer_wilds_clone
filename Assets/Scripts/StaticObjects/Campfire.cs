using PlayerLogic;
using UnityEngine;

namespace StaticObjects
{
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
