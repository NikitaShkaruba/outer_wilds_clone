using PlayerLogic.MarshmallowCookableParts;
using UnityEngine;

namespace StaticObjects.CampfireParts
{
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
