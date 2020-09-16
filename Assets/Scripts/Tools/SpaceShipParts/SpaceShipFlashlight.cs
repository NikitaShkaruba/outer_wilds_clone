using UnityEngine;

namespace Tools.SpaceShipParts
{
    public class SpaceShipFlashlight : MonoBehaviour
    {
        [SerializeField] private Light lightSource;

        [SerializeField] private Renderer interfaceIndicatorRenderer;
        [SerializeField] private Renderer lightBulbRenderer;

        [SerializeField] private Material nonLightedMaterial;
        [SerializeField] private Material lightedMaterial;

        public void Toggle()
        {
            if (lightSource.enabled)
            {
                lightSource.enabled = false;
                interfaceIndicatorRenderer.material = nonLightedMaterial;
                lightBulbRenderer.material = nonLightedMaterial;
            }
            else
            {
                lightSource.enabled = true;
                interfaceIndicatorRenderer.material = lightedMaterial;
                lightBulbRenderer.material = lightedMaterial;
            }
        }
    }
}
