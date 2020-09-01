using TMPro;
using UnityEngine;

namespace Debug
{
    public class TopLeftCornerDebug : MonoBehaviour
    {
        [SerializeField] private Player player;
        private TextMeshProUGUI textMeshPro;
    
        public bool isHidden;

        // Start is called before the first frame update
        private void Start()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            if (isHidden)
            {
                textMeshPro.text = "";
                return;
            }
        
            string playerVelocityText = GetPlayerVelocityText();
            string playerGravitatedTowardsText = GetPlayerGravitatedTowardsText();

            textMeshPro.text = playerVelocityText + playerGravitatedTowardsText;
        }

        private string GetPlayerVelocityText()
        {
            if (player.maxCelestialBody == null)
            {
                return "";
            }
        
            Vector3 playerVelocityCached = player.rigidbody.velocity;
            Vector3 maxCelestialBodyVelocity = player.maxCelestialBody.rigidbody.velocity;

            float playerVelocityX = playerVelocityCached.x - maxCelestialBodyVelocity.x;
            float playerVelocityY = playerVelocityCached.y - maxCelestialBodyVelocity.y;
            float playerVelocityZ = playerVelocityCached.z - maxCelestialBodyVelocity.z;
        
            const string stringFormat = "####0.00";
            string playerVelocityXText = playerVelocityX.ToString(stringFormat);
            string playerVelocityYText = playerVelocityY.ToString(stringFormat);
            string playerVelocityZText = playerVelocityZ.ToString(stringFormat);
        
            return $"Player velocity: ({playerVelocityXText}, {playerVelocityYText}, {playerVelocityZText})";
        }

        private string GetPlayerGravitatedTowardsText()
        {
            return player.maxCelestialBody ? $"\nGravitated towards: {player.maxCelestialBody.name}" : "";
        }
    }
}