using PlayerLogic.MarshmallowCookableParts;
using UnityEngine;

namespace StaticObjects.CampfireParts
{
    /**
     * Helps a marshmallow to understand if it should be heated or not
     */
    public class CampfireCookableHeat : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            Marshmallow marshmallow = other.GetComponent<Marshmallow>();
            if (marshmallow == null)
            {
                return;
            }

            marshmallow.Cook();
        }
    }
}
