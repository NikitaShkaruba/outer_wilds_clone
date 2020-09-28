using UnityEngine;
using UnityEngine.UI;

namespace PlayerTools.SpaceSuit
{
    [RequireComponent(typeof(Image))]
    public class SpaceSuitHealthIndicator : MonoBehaviour
    {
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void UpdatePercentage(float healthPercentage)
        {
            const float maxColorValue = 1f;
            float otherColorValues = 0.01f * healthPercentage * maxColorValue;

            image.color = new Color(maxColorValue, otherColorValues, otherColorValues);
        }
    }
}
