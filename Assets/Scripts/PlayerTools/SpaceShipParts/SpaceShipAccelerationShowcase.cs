using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

namespace PlayerTools.SpaceShipParts
{
    public class SpaceShipAccelerationShowcase : MonoBehaviour
    {
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material glowMaterial;

        private Vector3 acceleration;

        private Dictionary<string, Renderer[]> powerStepsRenderers;

        private const int FullPower = 5;
        private const int PartPower = 3;
        private const int NoPower = 0;

        private void Awake()
        {
            InitializeAccelerationStepRenderers();
        }

        private void Update()
        {
            LightDirection(acceleration.x, Directions.Back, Directions.Front);
            LightDirection(acceleration.z, Directions.Left, Directions.Right);
            LightDirection(acceleration.y, Directions.Bottom, Directions.Top, FullPower); // Yes, main game has FullPower too
        }

        public void UpdateInput(Vector3 updatedAcceleration)
        {
            acceleration = updatedAcceleration;
        }

        private void LightDirection(float accelerationValue, string positiveDirectionName, string negativeDirectionName, int forcedMinPower = 0)
        {
            int selectedPower = acceleration.x != 0 && acceleration.z != 0 ? PartPower : FullPower;
            if (forcedMinPower != 0)
            {
                selectedPower = forcedMinPower;
            }

            int positiveDirectionValue = NoPower;
            int negativeDirectionValue = NoPower;

            if (accelerationValue > 0)
            {
                positiveDirectionValue = selectedPower;
            }
            else if (accelerationValue < 0)
            {
                negativeDirectionValue = selectedPower;
            }

            LightUpDirection(positiveDirectionName, positiveDirectionValue);
            LightUpDirection(negativeDirectionName, negativeDirectionValue);
        }

        private void InitializeAccelerationStepRenderers()
        {
            powerStepsRenderers = new Dictionary<string, Renderer[]>();

            foreach (Transform directionTransform in transform)
            {
                powerStepsRenderers[directionTransform.name] = new Renderer[directionTransform.childCount];

                for (int accelerationPowerIndex = 0; accelerationPowerIndex < directionTransform.childCount; accelerationPowerIndex++)
                {
                    powerStepsRenderers[directionTransform.name][accelerationPowerIndex] = directionTransform.GetChild(accelerationPowerIndex).GetComponent<Renderer>();
                }
            }
        }

        private void LightUpDirection(string directionName, int lightedPower)
        {
            int lightedPowerIndex = lightedPower - 1;

            // We want to light up everything till power
            foreach (int powerIndex in Enumerable.Range(0, FullPower))
            {
                Renderer accelerationStepRenderer = powerStepsRenderers[directionName][powerIndex];
                accelerationStepRenderer.material = powerIndex <= lightedPowerIndex ? glowMaterial : defaultMaterial;
            }
        }
    }
}
