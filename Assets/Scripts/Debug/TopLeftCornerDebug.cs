using TMPro;
using UnityEngine;

namespace Debug
{
    public class TopLeftCornerDebug : MonoBehaviour
    {
        private TextMeshProUGUI textMeshPro;

        public bool isHidden;
        private static string additionalDebug;

        // Start is called before the first frame update
        private void Start()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

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