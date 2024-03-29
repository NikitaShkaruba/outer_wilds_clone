using PlayerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /**
     * Ui element that shows player information about it's health, oxygen, fuel and superFuel
     */
    public class SpaceSuitStatus : MonoBehaviour
    {
        [SerializeField] private Image healthIndicatorImage;
        [SerializeField] private Image oxygenBarFillerImage;
        [SerializeField] private Image fuelBarFillerImage;
        [SerializeField] private Image superFuelBarFillerImage;

        [SerializeField] private Player player;

        private void Start()
        {
            player.Damageable.OnHealthPointsChange += UpdateHealthIndicator;
            player.spaceSuit.OnOxygenTankFillPercentageChanged += UpdateOxygenBar;
            player.spaceSuit.OnFuelTankFillPercentageChanged += UpdateFuelBar;
            player.spaceSuit.OnSuperFuelTankFillPercentageChanged += UpdateSuperFuelBar;
        }

        private void OnDestroy()
        {
            player.Damageable.OnHealthPointsChange -= UpdateHealthIndicator;
            player.spaceSuit.OnOxygenTankFillPercentageChanged -= UpdateOxygenBar;
            player.spaceSuit.OnFuelTankFillPercentageChanged -= UpdateFuelBar;
            player.spaceSuit.OnSuperFuelTankFillPercentageChanged -= UpdateSuperFuelBar;
        }

        private void UpdateHealthIndicator(float percentage)
        {
            const float maxColorValue = 1f;
            float otherColorValues = 0.01f * percentage * maxColorValue;

            healthIndicatorImage.color = new Color(maxColorValue, otherColorValues, otherColorValues);
        }

        private void UpdateOxygenBar(float percentage)
        {
            UpdateBarFillerPercentage(oxygenBarFillerImage, percentage);
        }

        private void UpdateFuelBar(float percentage)
        {
            UpdateBarFillerPercentage(fuelBarFillerImage, percentage);
        }

        private void UpdateSuperFuelBar(float percentage)
        {
            UpdateBarFillerPercentage(superFuelBarFillerImage, percentage);
        }

        private static void UpdateBarFillerPercentage(Image barFillerImage, float percentage)
        {
            barFillerImage.fillAmount = percentage / 100 * 0.25f;
        }
    }
}
