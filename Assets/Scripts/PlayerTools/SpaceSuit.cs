using PlayerLogic;

namespace PlayerTools
{
    public class SpaceSuit
    {
        // Oxygen
        public readonly Tank OxygenTank;
        private const float OxygenDepletionSpeed = 0.01f;
        private const float OxygenRefillSpeed = 0.5f;

        // Fuel
        public readonly Tank FuelTank;
        private const float FuelDepletionSpeed = 0.01f;
        private const float FuelRefillSpeed = 2f;

        // Super-Fuel
        public readonly Tank SuperFuelTank;
        private const float SuperFuelDepletionSpeed = 1f;
        private const float SuperFuelRestorationSpeed = 0.4f;
        private const float SuperFuelPowerMultiplier = 2f;
        private bool isUsingSuperFuel;

        // Thrusters
        private const float ThrustersPower = 500f;
        private const float NoThrustersPower = 0f;

        public SpaceSuit()
        {
            OxygenTank = new Tank();
            FuelTank = new Tank();
            SuperFuelTank = new Tank();
        }

        public void Tick()
        {
            if (!isUsingSuperFuel && !SuperFuelTank.IsFull && HasPropellant())
            {
                FillSuperFuelTank();
            }
        }

        public bool GiveOxygenToBreathe()
        {
            if (OxygenTank.IsEmpty)
            {
                return false;
            }

            OxygenTank.Deplete(OxygenDepletionSpeed);

            return true;
        }

        public float FireVerticalThrusters(bool useSuperFuel)
        {
            if (!HasPropellant())
            {
                isUsingSuperFuel = false;
                return NoThrustersPower;
            }

            if (!useSuperFuel || SuperFuelTank.IsEmpty)
            {
                isUsingSuperFuel = false;
                DepletePropellant();

                return ThrustersPower;
            }

            DepletePropellant();
            SuperFuelTank.Deplete(SuperFuelDepletionSpeed);
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
            FuelTank.Fill(FuelRefillSpeed);
        }

        public void FillOxygenTank()
        {
            OxygenTank.Fill(OxygenRefillSpeed);
        }

        private bool HasPropellant()
        {
            return !FuelTank.IsEmpty || !OxygenTank.IsEmpty;
        }

        private void DepletePropellant()
        {
            if (!FuelTank.IsEmpty)
            {
                FuelTank.Deplete(FuelDepletionSpeed);
            }
            else if (!OxygenTank.IsEmpty)
            {
                OxygenTank.Deplete(OxygenDepletionSpeed);
            }
        }

        private void FillSuperFuelTank()
        {
            DepletePropellant();

            SuperFuelTank.Fill(SuperFuelRestorationSpeed);
        }
    }
}
