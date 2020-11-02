using PlayerLogic.MarshmallowCookableParts;
using UnityEngine;

namespace StaticObjects.CampfireParts
{
    /**
     * Helps a marshmallow to understand if it should be burned or not
     */
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
