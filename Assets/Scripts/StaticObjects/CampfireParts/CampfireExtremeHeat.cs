using PlayerTools;
using UnityEngine;

namespace StaticObjects.CampfireParts
{
    public class CampfireExtremeHeat : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            Marshmallow marshmallow = other.GetComponent<Marshmallow>();
            if (marshmallow == null)
            {
                return;
            }

            marshmallow.Burn();
        }
    }
}
