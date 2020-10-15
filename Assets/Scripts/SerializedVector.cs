using System;
using UnityEngine;

[Serializable]
public class SerializedVector
{
    public float x;
    public float y;
    public float z;

    public SerializedVector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Vector3(SerializedVector serializedVector)
    {
        return new Vector3(serializedVector.x, serializedVector.y, serializedVector.z);
    }

    public static implicit operator SerializedVector(Vector3 vector)
    {
        return new SerializedVector(vector.x, vector.y, vector.z);
    }

    public override string ToString()
    {
        return $"[{x}, {y}, {z}]";
    }
}