using UnityEngine;

namespace UI
{
    public class SpaceNavigator : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject suggestCursor;
        [SerializeField] private GameObject lockCursor;

        private CelestialBody lockedCelestialBody;

        private void Update()
        {
            ProcessUserInput();
        }

        private void FixedUpdate()
        {
            if (!IsLocked())
            {
                TryToSuggestCelestialBody();
            }
            else
            {
                UpdateLockCoordinates();
            }
        }
        
        private void ProcessUserInput()
        {
            bool mouseLeftClicked = Input.GetMouseButtonDown(0);
            if (!mouseLeftClicked)
            {
                return;
            }

            if (HasSuggestedPlanet() && !IsLocked())
            {
                Lock();
            }
            else if (IsLocked())
            {
                Unlock();
            }
        }

        private void TryToSuggestCelestialBody()
        {
            if (IsLocked())
            {
                return;
            }
            
            CelestialBody suggestedCelestialBody = GetSuggestedCelestialBody();

            if (suggestedCelestialBody != null)
            {
                UpdateCursorCoordinates(suggestedCelestialBody);
                suggestCursor.SetActive(true);
            }
            else
            {
                suggestCursor.SetActive(false);
            }
        }

        private void UpdateLockCoordinates()
        {
            UpdateCursorCoordinates(lockedCelestialBody);
        }

        private CelestialBody GetSuggestedCelestialBody()
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

        private bool HasSuggestedPlanet()
        {
            return suggestCursor.activeSelf;
        }

        private void UpdateCursorCoordinates(CelestialBody celestialBody)
        {
            // Suggest this planet
            Vector3 worldToScreenPoint = camera.WorldToScreenPoint(celestialBody.Position);
            worldToScreenPoint.z /= 10; // If Z coordinate is too big, we don't see the cursor. This reduces it's coordinate

            transform.position = worldToScreenPoint;
        }
        
        private void Lock()
        {
            lockedCelestialBody = GetSuggestedCelestialBody();
            suggestCursor.SetActive(false);
            lockCursor.SetActive(true);
        }

        private void Unlock()
        {
            lockCursor.SetActive(false);
            lockedCelestialBody = null;
        }

        private bool IsLocked()
        {
            return lockedCelestialBody != null;
        }
    }
}