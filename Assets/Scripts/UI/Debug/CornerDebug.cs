using System.Collections.Generic;
using PlayerLogic;
using TMPro;
using UnityEngine;

namespace UI.Debug
{
    /**
     * UI element that shows debug information at the bottom left corner of a game
     */
    public class CornerDebug : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private PlayerControllable playerControllable;

        public bool isHidden;
        private static string additionalDebug;
        private static readonly List<string> GravityDebugCelestialBodies = new List<string>();

        private void Awake()
        {
            playerControllable.OnCornerDebugToggle += Toggle;
        }

        private void OnDestroy()
        {
            playerControllable.OnCornerDebugToggle -= Toggle;
        }

        private void FixedUpdate()
        {
            textMeshPro.text = !isHidden ? additionalDebug : "";
            ResetDebug();
        }

        private void Toggle()
        {
            isHidden = !isHidden;
        }

        public static void AddDebug(string debugString)
        {
            additionalDebug += debugString + "\n";
        }

        public static void AddGravityDebug(string celestialBodyName, string debugString)
        {
            // We don't want to write it too many times in fixedUpdate
            if (GravityDebugCelestialBodies.Contains(celestialBodyName))
            {
                return;
            }

            GravityDebugCelestialBodies.Add(celestialBodyName);
            AddDebug(debugString);
        }

        private static void ResetDebug()
        {
            additionalDebug = "";
            GravityDebugCelestialBodies.RemoveAll(_ => true);
            AddDefaultDebug();
        }

        private static void AddDefaultDebug()
        {
            float secondsSinceStart = Time.time;
            const int secondsInMinute = 60;

            int minutesSinceStart = (int) secondsSinceStart / secondsInMinute;
            int seconds = (int) secondsSinceStart - secondsInMinute * minutesSinceStart;

            AddDebug($"Time: {minutesSinceStart:D2}:{seconds:D2}");
        }
    }
}
