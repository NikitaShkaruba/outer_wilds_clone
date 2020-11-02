using PlayerLogic;
using TMPro;
using UnityEngine;

namespace UI
{
    /**
     * Ui element that shows the player that he is being damaged
     */
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class YouAreTakingDamageText : MonoBehaviour
    {
        [SerializeField] private string text;
        [SerializeField] private float timeToShow;

        private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Player player;

        private float timer;

        private void Awake()
        {
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            player.Damageable.OnTakingDamage += Enable;
        }

        private void OnDestroy()
        {
            player.Damageable.OnTakingDamage -= Enable;
        }

        private void FixedUpdate()
        {
            if (!IsEnabled())
            {
                return;
            }

            CheckTextForTimeout();
        }

        private void CheckTextForTimeout()
        {
            timer -= Time.deltaTime;

            if (!IsEnabled())
            {
                textMeshProUGUI.text = "";
            }
        }

        private void Enable()
        {
            timer = timeToShow;
            textMeshProUGUI.text = text;
        }

        private bool IsEnabled()
        {
            return timer > 0;
        }
    }
}
