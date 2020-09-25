using UnityEngine;
using UnityEngine.UI;

namespace Tools.SpaceSuit
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
            const float maxColorValue = 256f;
            float otherColorValues = 0.0001f * healthPercentage * maxColorValue;

            image.color = new Color(maxColorValue, otherColorValues, otherColorValues);
        }
    }
}
