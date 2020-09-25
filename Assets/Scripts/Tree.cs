using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Tree : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Player collidedPlayer = other.GetComponent<Player>();
        if (collidedPlayer == null)
        {
            return;
        }

        collidedPlayer.FillOxygenTanks();
    }
}
