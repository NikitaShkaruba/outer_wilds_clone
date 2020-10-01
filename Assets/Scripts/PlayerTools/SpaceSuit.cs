using System;
using PlayerLogic;

namespace PlayerTools
{
    public class SpaceSuit
    {
        // Oxygen
        private readonly Tank oxygenTank;
        private const float OxygenDepletionSpeed = 0.01f;
        private const float OxygenRefillSpeed = 0.5f;

        // Fuel
        private readonly Tank fuelTank;
        private const float FuelDepletionSpeed = 0.01f;
        private const float FuelRefillSpeed = 2f;

        // Super-Fuel
        private readonly Tank superFuelTank;
        private const float SuperFuelDepletionSpeed = 1f;
        private const float SuperFuelRestorationSpeed = 0.4f;
        private const float SuperFuelPowerMultiplier = 2f;
        private bool isUsingSuperFuel;

        // Thrusters
        private const float ThrustersPower = 500f;
        private const float NoThrustersPower = 0f;

        // Handy properties
        public bool IsFuelTankFull => fuelTank.IsFull;

        // UI events
        public event Action<float> OnOxygenTankFillPercentageChanged;
        public event Action<float> OnFuelTankFillPercentageChanged;
        public event Action<float> OnSuperFuelTankFillPercentageChanged;

        public SpaceSuit()
        {
            oxygenTank = new Tank();
            fuelTank = new Tank();
            superFuelTank = new Tank();

            oxygenTank.FillPercentageChanged += InvokeOnOxygenTankFillPercentageChanged;
            fuelTank.FillPercentageChanged += InvokeOnFuelTankFillPercentageChanged;
            superFuelTank.FillPercentageChanged += InvokeOnSuperFuelTankFillPercentageChanged;
        }

        ~SpaceSuit()
        {
            oxygenTank.FillPercentageChanged -= InvokeOnOxygenTankFillPercentageChanged;
            fuelTank.FillPercentageChanged -= InvokeOnFuelTankFillPercentageChanged;
            superFuelTank.FillPercentageChanged -= InvokeOnSuperFuelTankFillPercentageChanged;
        }

        public void Tick()
        {
            if (!isUsingSuperFuel && !superFuelTank.IsFull && HasPropellant())
            {
                FillSuperFuelTank();
            }
        }

        public bool GiveOxygenToBreathe()
        {
            if (oxygenTank.IsEmpty)
            {
                return false;
            }

            oxygenTank.Deplete(OxygenDepletionSpeed);

            return true;
        }

        public float FireVerticalThrusters(bool useSuperFuel)
        {
            if (!HasPropellant())
            {
                isUsingSuperFuel = false;
                return NoThrustersPower;
            }

            if (!useSuperFuel || superFuelTank.IsEmpty)
            {
                isUsingSuperFuel = false;
                DepletePropellant();

                return ThrustersPower;
            }

            DepletePropellant();
            superFuelTank.Deplete(SuperFuelDepletionSpeed);
            isUsingSuperFuel = true;

            return SuperFuelPowerMultiplier * ThrustersPower;
        }

        public float FireHorizontalThrusters()
        {
            if (!HasPropellant())
            {
                return NoThrustersPower;
            }

            DepletePropellant();

            return ThrustersPower;
        }

        public void FillFuelTank()
        {
            fuelTank.Fill(FuelRefillSpeed);
        }

        public void FillOxygenTank()
        {
            oxygenTank.Fill(OxygenRefillSpeed);
        }

        private bool HasPropellant()
        {
            return !fuelTank.IsEmpty || !oxygenTank.IsEmpty;
        }

        private void DepletePropellant()
        {
            if (!fuelTank.IsEmpty)
            {
                fuelTank.Deplete(FuelDepletionSpeed);
            }
            else if (!oxygenTank.IsEmpty)
            {
                oxygenTank.Deplete(OxygenDepletionSpeed);
            }
        }

        private void FillSuperFuelTank()
        {
            DepletePropellant();

            superFuelTank.Fill(SuperFuelRestorationSpeed);
        }

        private void InvokeOnSuperFuelTankFillPercentageChanged(float percentage)
        {
            OnSuperFuelTankFillPercentageChanged?.Invoke(percentage);
        }

        private void InvokeOnFuelTankFillPercentageChanged(float percentage)
        {
            OnFuelTankFillPercentageChanged?.Invoke(percentage);
        }

        private void InvokeOnOxygenTankFillPercentageChanged(float percentage)
        {
            OnOxygenTankFillPercentageChanged?.Invoke(percentage);
        }
    }
}
