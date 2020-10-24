using Celestial;
using UI.Debug;
using UnityEngine;

namespace Physics
{
    public class Gravitatable
    {
        private readonly Rigidbody rigidbody;
        private readonly bool isCelestialBody;

        private readonly CelestialBody[] celestialBodies;
        private MaxGravitatableInfo maxGravitatableInfo;

        public Gravitatable(Rigidbody rigidbody, CelestialBody[] celestialBodies, bool isCelestialBody = false)
        {
            this.rigidbody = rigidbody;
            this.isCelestialBody = isCelestialBody;
            this.celestialBodies = celestialBodies;
        }

        public MaxGravitatableInfo ApplyGravity()
        {
            maxGravitatableInfo = new MaxGravitatableInfo();

            foreach (CelestialBody celestialBody in celestialBodies)
            {
                // This difference is needed because celestial body gravity is a little bit to harsh for the player
                Vector3 gravityForce = isCelestialBody ? Gravitation.ComputeCelestialBodyForce(rigidbody, celestialBody.rigidbody) : Gravitation.ComputeNonCelestialBodyForce(rigidbody, celestialBody);
                gravityForce *= Time.deltaTime;
                rigidbody.AddForce(gravityForce);

                if (gravityForce.magnitude > maxGravitatableInfo.MaxGravityForce.magnitude)
                {
                    maxGravitatableInfo.Update(gravityForce, celestialBody);
                }

                CornerDebug.AddGravityDebug(celestialBody.name, $"'{celestialBody.name}' gravity magnitude: {gravityForce.magnitude}");
            }

            return maxGravitatableInfo;
        }
    }
}
