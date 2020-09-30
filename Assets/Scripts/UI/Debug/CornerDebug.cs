using PlayerLogic;
using TMPro;
using UnityEngine;

namespace UI.Debug
{
    public class CornerDebug : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private PlayerInput playerInput;

        public bool isHidden;
        private static string additionalDebug;

        private void Awake()
        {
            playerInput.onCornerDebugToggle += Toggle;
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
