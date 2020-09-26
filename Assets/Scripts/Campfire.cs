using UnityEngine;

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

        collidedPlayer.Hurt(fireDamage);
    }
}
