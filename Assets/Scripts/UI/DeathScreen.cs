using PlayerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class DeathScreen : MonoBehaviour
    {
        private const float FadingSpeed = 0.005f;

        private Image deathBlackFadeImage;
        [SerializeField] private Player player;

        private new bool enabled;

        public void Awake()
        {
            deathBlackFadeImage = GetComponent<Image>();
        }

        public void Start()
        {
            player.Dieable.OnDeath += Enable;
        }

        public void OnDestroy()
        {
            player.Dieable.OnDeath -= Enable;
        }

        public void Update()
        {
            if (!enabled)
            {
                return;
            }

            FadeScreen();
        }

        private void Enable()
        {
            enabled = true;
        }

        private void FadeScreen()
        {
            Color nextDeathBlackFadeImage = deathBlackFadeImage.color;
            nextDeathBlackFadeImage.a += FadingSpeed;

            deathBlackFadeImage.color = nextDeathBlackFadeImage;
        }
    }
}
