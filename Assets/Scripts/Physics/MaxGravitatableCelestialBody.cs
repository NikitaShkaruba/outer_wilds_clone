using Celestial;
using UnityEngine;

namespace Physics
{
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
