using UnityEngine;
using UnityEngine.UI;

namespace UI.SpaceSuitStatus
{
    [RequireComponent(typeof(Image))]
    public class SpaceSuitBar : MonoBehaviour
    {
        private Image barImage;

        private void Awake()
        {
            barImage = GetComponent<Image>();
        }

        public void UpdatePercentage(float percentage)
        {
            barImage.fillAmount = percentage / 100 * 0.25f;
        }
    }
}
