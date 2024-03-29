using System.Collections.Generic;
using UnityEngine;

namespace Celestial
{
    /**
     * Class for an orbit trail - stores celestial body orbit coordinates and draws them
     */
    public class Orbit
    {
        private readonly LinkedList<Vector3> coordinates = new LinkedList<Vector3>();
        private const int StepSize = 100;
        private const int MaxLength = 2000;
        private readonly Color color;

        public Orbit(Vector3 startPosition, Color color)
        {
            coordinates.AddLast(startPosition);
            this.color = color;
        }

        public void Draw()
        {
            LinkedListNode<Vector3> element = coordinates.First;
            LinkedListNode<Vector3> nextElement = element.Next;

            while (nextElement != coordinates.Last && nextElement != null)
            {
                Debug.DrawLine(element.Value, nextElement.Value, color);

                element = nextElement;
                nextElement = element.Next;
            }
        }

        public void Update(CelestialBody celestialBody)
        {
            // Don't update coordinates if difference in coordinates is low
            if ((coordinates.Last.Value - celestialBody.rigidbody.position).magnitude < StepSize)
            {
                return;
            }

            // Add new position
            coordinates.AddLast(celestialBody.rigidbody.position);
            if (coordinates.Count == MaxLength)
            {
                coordinates.RemoveFirst();
            }
        }
    }
}
