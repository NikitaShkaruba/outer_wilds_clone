using UnityEngine;

namespace UI
{
    public class SpaceNavigator : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject suggestedLock;

        private void FixedUpdate()
        {
            TryToSuggestLockForPlanet();
        }

        private void TryToSuggestLockForPlanet()
        {
            CelestialBody suggestedToLockPlanet = GetSuggestedToLockPlanet();

            if (suggestedToLockPlanet != null)
            {
                Lock(suggestedToLockPlanet);
                Enable();
            }
            else
            {
                Disable();
            }
        }

        private CelestialBody GetSuggestedToLockPlanet()
        {
            Transform cachedCameraTransform = camera.transform;

            // Try to find something at the cursor
            bool foundSomething = Physics.Raycast(cachedCameraTransform.position, cachedCameraTransform.forward, out RaycastHit raycastHit);
            if (!foundSomething)
            {
                return null;
            }

            // We only need celestialBody
            CelestialBody celestialBody = raycastHit.transform.GetComponent<CelestialBody>();
            if (celestialBody == null)
            {
                return null;
            }

            return celestialBody;
        }

        private void Enable()
        {
            suggestedLock.SetActive(true);
        }

        private void Disable()
        {
            suggestedLock.SetActive(false);
        }

        private void Lock(CelestialBody celestialBody)
        {
            // Suggest this planet
            Vector3 worldToScreenPoint = camera.WorldToScreenPoint(celestialBody.Position);
            worldToScreenPoint.z /= 10; // If Z coordinate is too big, we don't see the cursor. This reduces it's coordinate

            transform.position = worldToScreenPoint;
        }
    }
}