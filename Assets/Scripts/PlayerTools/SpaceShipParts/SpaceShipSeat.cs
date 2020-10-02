using Common;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipSeat : MonoBehaviour
    {
        public SpaceShip spaceShip;
        public Player seatedPlayer;
        private bool buckleUpTransitionGoing;
        public SpaceShipInterface spaceShipInterface;

        public void FixedUpdate()
        {
            if (seatedPlayer == null)
            {
                return;
            }

            if (buckleUpTransitionGoing)
            {
                DoBucklingUpTransitionPiece();
            }
        }

        public void StartBucklingUp(Player player)
        {
            seatedPlayer = player;

            // Change state
            buckleUpTransitionGoing = true;
            seatedPlayer.transform.SetParent(transform);

            // Disable movement
            seatedPlayer.rigidbody.isKinematic = true;
            seatedPlayer.rigidbody.detectCollisions = false;
        }

        public void Unbuckle()
        {
            // Enable movement again
            seatedPlayer.rigidbody.velocity = spaceShip.rigidbody.velocity;
            seatedPlayer.rigidbody.isKinematic = false;
            seatedPlayer.rigidbody.detectCollisions = true;

            // Change state
            seatedPlayer.transform.SetParent(null);
            buckleUpTransitionGoing = false; // In case we stopped buckling up during the transition
        }

        private void DoBucklingUpTransitionPiece()
        {
            Transform cachedPlayerTransform = seatedPlayer.transform;
            Vector3 transformCachedPosition = cachedPlayerTransform.localPosition;
            Quaternion cachedTransformRotation = cachedPlayerTransform.localRotation;
            Quaternion cachedCameraTransformRotation = seatedPlayer.camera.transform.localRotation;

            // Move player into the chair
            Vector3 desiredPosition = new Vector3(0, 0.5f, 1.1f); // A little bit forward and up of the (0,0,0) coordinates of the chair
            Vector3 positionDifference = desiredPosition - transformCachedPosition;
            Vector3 positionAddition = positionDifference; // difference - is what we should add in order to become the same
            positionAddition *= 2f; // Speedup the process a bit
            positionAddition *= Time.deltaTime; // Include physics rendering step
            transformCachedPosition += positionAddition;
            cachedPlayerTransform.localPosition = transformCachedPosition;

            // Rotate player body to (0,0,0)
            Quaternion wantedTransformRotation = Quaternion.Euler(BlenderBugFixes.TransformBlenderEulerAngles(new Vector3(0, 0, 0)));
            cachedTransformRotation = Quaternion.Slerp(cachedTransformRotation, wantedTransformRotation, Time.deltaTime);
            cachedPlayerTransform.localRotation = cachedTransformRotation;

            // Rotate player camera to (0,0,0)
            Quaternion wantedCameraRotation = Quaternion.Euler(0, 0, 0);
            cachedCameraTransformRotation = Quaternion.Slerp(cachedCameraTransformRotation, wantedCameraRotation, Time.deltaTime);

            seatedPlayer.camera.transform.localRotation = cachedCameraTransformRotation;

            // Check that everything is positioned properly
            const float positionCheckThreshold = 0.01f;
            bool movedToTheChair = Mathf.Abs(positionDifference.x) <= positionCheckThreshold &&
                                   Mathf.Abs(positionDifference.y) <= positionCheckThreshold &&
                                   Mathf.Abs(positionDifference.z) <= positionCheckThreshold;

            // Check that everything is rotated properly
            const float rotationCheckThreshold = 0.00001f;
            bool transformRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(cachedTransformRotation, wantedTransformRotation)) < rotationCheckThreshold;
            bool cameraRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(cachedCameraTransformRotation, wantedCameraRotation)) < rotationCheckThreshold;

            if (movedToTheChair && transformRotatedProperly && cameraRotatedProperly)
            {
                buckleUpTransitionGoing = false;
            }
        }
    }
}
