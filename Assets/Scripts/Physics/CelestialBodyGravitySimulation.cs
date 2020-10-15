using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace Physics
{
    public static class CelestialBodyGravitySimulation
    {
        private static Dictionary<string, List<SerializedVector>> memorizedCoordinates = new Dictionary<string, List<SerializedVector>>();
        private static readonly Dictionary<string, int> CelestialBodyIndexes = new Dictionary<string, int>();

        static CelestialBodyGravitySimulation()
        {
            ReadStoredCoordinatesFromDisk();
        }

        public static void Register(string celestialBodyName)
        {
            CelestialBodyIndexes[celestialBodyName] = 0;
        }

        private static void ReadStoredCoordinatesFromDisk()
        {
            memorizedCoordinates = new Dictionary<string, List<SerializedVector>>();

            string[] fileNames =
            {
                // "sun_station_movement.dat",
                // "twins_movement.dat",
                "timber_hearth_movement.dat"
            };

            foreach (string fileName in fileNames)
            {
                using (Stream stream = File.Open(@"C:\Users\Public\" + fileName, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    var newMemorizedCoordinates = (Dictionary<string, List<SerializedVector>>) binaryFormatter.Deserialize(stream);

                    foreach (string planetName in newMemorizedCoordinates.Keys)
                    {
                        memorizedCoordinates[planetName] = newMemorizedCoordinates[planetName];
                        Debug.Log($"Read '{fileName}' contents");
                    }
                }
            }
        }

        public static Vector3 GiveNextPosition(string name)
        {
            int positionIndex = CelestialBodyIndexes[name]++;

            return memorizedCoordinates[name][positionIndex];
        }

        [MenuItem("Tools/PutMemorizedCoordinatesToFile")]
        public static void PutMemorizedCoordinatesToFile()
        {
            FileStream memorizedCoordinatesFile = File.Create(@"C:\Users\Public\celestial_bodies_movement.dat");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memorizedCoordinatesFile, memorizedCoordinates);

            memorizedCoordinatesFile.Flush(true);
        }

        public static void StoreCoordinate(string name, Vector3 position)
        {
            if (name == "Timber Hearth")
            {
                if (!memorizedCoordinates.ContainsKey(name))
                {
                    memorizedCoordinates[name] = new List<SerializedVector>();
                }

                memorizedCoordinates[name].Add(position);
            }
        }
    }
}
