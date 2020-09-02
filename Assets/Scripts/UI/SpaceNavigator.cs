using TMPro;
using UnityEngine;

namespace UI
{
    public class SpaceNavigator : MonoBehaviour
    {
        // Suggest cursor
        [SerializeField] private GameObject suggestCursor;
        
        // Lock cursor
        [SerializeField] private GameObject lockCursor;
        [SerializeField] private TextMeshProUGUI lockInformation;
        [SerializeField] private GameObject topVelocityArrow;
        [SerializeField] private GameObject rightVelocityArrow;
        [SerializeField] private GameObject bottomVelocityArrow;
        [SerializeField] private GameObject leftVelocityArrow;

        // Linked objects
        [SerializeField] private Player player;
        
        private CelestialBody lockedCelestialBody;
        private float previousDistance;

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
                UpdateLock();
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

        private bool HasSuggestedPlanet()
        {
            return suggestCursor.activeSelf;
        }

        private void TryToSuggestCelestialBody()
        {
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

        private CelestialBody GetSuggestedCelestialBody()
        {
            Transform cachedCameraTransform = player.camera.transform;

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

        private void UpdateLock()
        {
            UpdateCursorCoordinates(lockedCelestialBody);
            UpdateLockText();
            UpdateVelocityArrows();
        }

        private void UpdateCursorCoordinates(CelestialBody celestialBody)
        {
            // Suggest this planet
            Vector3 worldToScreenPoint = player.camera.WorldToScreenPoint(celestialBody.Position);
            worldToScreenPoint.z /= 10; // If Z coordinate is too big, we don't see the cursor. This reduces it's coordinate

            transform.position = worldToScreenPoint;
        }
        
        private void UpdateLockText()
        {
            string celestialBodyName = lockedCelestialBody.name;
            string distanceToCelestialBody = GetDistanceToCelestialBodyText();
            string velocityToCelestialBody = GetVelocityMagnitudeToCelestialBodyText();

            lockInformation.text = $"{celestialBodyName}\n{distanceToCelestialBody}\n{velocityToCelestialBody}";
        }

        private void UpdateVelocityArrows()
        {
            Vector3 velocityDifference = player.rigidbody.velocity - lockedCelestialBody.rigidbody.velocity;

            Vector3 playerEulers = player.transform.rotation.eulerAngles;
            velocityDifference = Quaternion.Euler(0, 0, -playerEulers.z) * velocityDifference;

            rightVelocityArrow.transform.localPosition = new Vector3(GetArrowLocalPosition(velocityDifference.x), 0f, 0f);
            leftVelocityArrow.transform.localPosition = new Vector3(GetArrowLocalPosition(-velocityDifference.x), 0f, 0f);
            topVelocityArrow.transform.localPosition = new Vector3(GetArrowLocalPosition(velocityDifference.y), 0f, 0f);
            bottomVelocityArrow.transform.localPosition = new Vector3(GetArrowLocalPosition(-velocityDifference.y), 0f, 0f);
        }

        /**
         * Arrow moving is implemented with a mask where arrow totally hidden by mask and gets revealed slowly
         */
        private static float GetArrowLocalPosition(float velocityCoordinateDifference)
        {
            const int nothingIsViewedValue = -100;
            const int scaleFactor = 100;

            float value = velocityCoordinateDifference / scaleFactor;

            return nothingIsViewedValue + value;
        }

        private string GetDistanceToCelestialBodyText()
        {
            float metersFromPlayerToCelestialBody = (player.Position - lockedCelestialBody.Position).magnitude;

            if (metersFromPlayerToCelestialBody > 5000)
            {
                return (metersFromPlayerToCelestialBody / 1000).ToString("####0") + "km";
            }
            else
            {
                return metersFromPlayerToCelestialBody.ToString("####0") + "m";
            }
        }

        private string GetVelocityMagnitudeToCelestialBodyText()
        {
            float velocityToCelestialBody = (player.rigidbody.velocity - lockedCelestialBody.rigidbody.velocity).magnitude;

            float currentDistance = (player.Position - lockedCelestialBody.Position).magnitude;
            string velocitySign = previousDistance < currentDistance ? "-" : "";
            previousDistance = currentDistance;

            return $"{velocitySign}{velocityToCelestialBody:####0}m/s";
        }
    }
}