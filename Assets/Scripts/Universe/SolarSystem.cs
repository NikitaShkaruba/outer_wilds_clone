using UnityEngine;

namespace Universe
{
    public class SolarSystem : MonoBehaviour
    {
        public static CelestialBody[] CelestialBodies;

        private void Awake()
        {
            CelestialBodies = FindObjectsOfType<CelestialBody>();
        }
    }
}
