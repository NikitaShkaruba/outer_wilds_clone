using System.Collections.Generic;
using UnityEngine;

public class Orbit
{
    private readonly LinkedList<Vector3> coordinates = new LinkedList<Vector3>();
    private const int StepSize = 100;
    private const int MaxLength = 2000;

    public Orbit(Vector3 startPosition)
    {
        coordinates.AddLast(startPosition);
    }

    public void Draw()
    {
        LinkedListNode<Vector3> element = coordinates.First;
        LinkedListNode<Vector3> nextElement = element.Next;
        
        while (nextElement != coordinates.Last && nextElement != null)
        {
            Debug.DrawLine(element.Value, nextElement.Value);

            element = nextElement;
            nextElement = element.Next;
        }
    }

    public void Update(CelestialBody celestialBody)
    {
        // Don't update coordinates if difference in coordinates is low
        if ((coordinates.Last.Value - celestialBody.Position).magnitude < StepSize)
        {
            return;
        }
        
        // Add new position
        coordinates.AddLast(celestialBody.Position);
        if (coordinates.Count == MaxLength)
        {
            coordinates.RemoveFirst();
        }
    }
}