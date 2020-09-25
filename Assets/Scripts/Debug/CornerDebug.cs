using TMPro;
using UnityEngine;

namespace Debug
{
    public class CornerDebug : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        public bool isHidden;
        private static string additionalDebug;

        private void FixedUpdate()
        {
            textMeshPro.text = !isHidden ? additionalDebug : "";
            ResetDebug();
        }

        public static void AddDebug(string debugString)
        {
            additionalDebug += debugString + "\n";
        }

        private static void ResetDebug()
        {
            additionalDebug = "";
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
