using TMPro;
using UnityEngine;

namespace Debug
{
    public class TopLeftCornerDebug : MonoBehaviour
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
        }
    }
}
