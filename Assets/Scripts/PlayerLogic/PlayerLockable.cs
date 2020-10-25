using UnityEngine;

namespace PlayerLogic
{
    public class PlayerLockable
    {
        private bool isTransitionGoing;
        private Player player;
        private Vector3 wantedLocalPosition;
        private Quaternion wantedLocalBodyRotation;
        private Quaternion wantedLocalCameraRotation;

        public bool IsLocked => player != null;

        public void Lock(Player player, Transform transformToLockOn, Vector3 wantedLocalPosition, Quaternion wantedLocalBodyRotation, Quaternion wantedLocalCameraRotation)
        {
            this.player = player;
            this.wantedLocalPosition = wantedLocalPosition;
            this.wantedLocalBodyRotation = wantedLocalBodyRotation;
            this.wantedLocalCameraRotation = wantedLocalCameraRotation;
            isTransitionGoing = true;

            player.transform.SetParent(transformToLockOn);
            player.rigidbody.isKinematic = true;
            player.rigidbody.detectCollisions = false;
        }

        public void Unlock(Vector3 newVelocity)
        {
            player.rigidbody.velocity = newVelocity;
            player.rigidbody.isKinematic = false;
            player.rigidbody.detectCollisions = true;
            player.transform.SetParent(null);

            player = null;
            wantedLocalPosition = Vector3.zero;
            wantedLocalBodyRotation = new Quaternion();
            wantedLocalCameraRotation = new Quaternion();
            isTransitionGoing = false; // In case we stopped locking during transition
        }

        public void Process()
        {
            if (!isTransitionGoing)
            {
                return;
            }

            // Move and rotate the player
            player.transform.localPosition = SlerpPosition(player.transform.localPosition, wantedLocalPosition);
            player.transform.localRotation = Quaternion.Slerp(player.transform.localRotation, wantedLocalBodyRotation, Time.deltaTime);
            player.camera.transform.localRotation = Quaternion.Slerp(player.camera.transform.localRotation, wantedLocalCameraRotation, Time.deltaTime);

            if (IsTransitionFinished())
            {
                isTransitionGoing = false;
            }
        }

        private static Vector3 SlerpPosition(Vector3 currentTransformPosition, Vector3 wantedLocalPosition)
        {
            Vector3 positionAddition = wantedLocalPosition - currentTransformPosition; // Difference - is what we should add in order to become the same
            positionAddition *= 2f; // Speedup the process a bit
            positionAddition *= Time.deltaTime; // Include physics rendering step
            currentTransformPosition += positionAddition;

            return currentTransformPosition;
        }

        private bool IsTransitionFinished()
        {
            // Check position
            const float positionCheckThreshold = 0.01f;
            Vector3 positionDifference = wantedLocalPosition - player.transform.localPosition;
            bool movedToTheChair = Mathf.Abs(positionDifference.x) <= positionCheckThreshold &&
                                   Mathf.Abs(positionDifference.y) <= positionCheckThreshold &&
                                   Mathf.Abs(positionDifference.z) <= positionCheckThreshold;

            // Check rotation
            const float rotationCheckThreshold = 0.00001f;
            bool transformRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(player.transform.localRotation, wantedLocalBodyRotation)) < rotationCheckThreshold;
            bool cameraRotatedProperly = 1 - Mathf.Abs(Quaternion.Dot(player.camera.transform.localRotation, wantedLocalCameraRotation)) < rotationCheckThreshold;

            return movedToTheChair && transformRotatedProperly && cameraRotatedProperly;
        }
    }
}
