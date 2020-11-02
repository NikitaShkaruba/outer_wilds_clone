using Celestial;
using UnityEngine;

namespace Physics
{
    /**
     * Class that stores information about max gravitatable celestial body, which can later be used to determine
     * around which celestial body you should rotate the player or the spaceShip
     */
    public class MaxGravitatableInfo
    {
        public Vector3 MaxGravityForce;
        public CelestialBody CelestialBody;

        public MaxGravitatableInfo()
        {
            MaxGravityForce = Vector3.zero;
            CelestialBody = null;
        }

        public void Update(Vector3 maxGravityForce, CelestialBody celestialBody)
        {
            MaxGravityForce = maxGravityForce;
            CelestialBody = celestialBody;
        }
    }
}
