using UnityEngine;

namespace PlayerLogic.MarshmallowCookableParts
{
    /**
     * Class that represents marshmallow.
     * It knows what is it to be cooked (logic to update it's color), it knows how to be burned, and it knows how to self-destruct when not needed
     */
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Marshmallow : MonoBehaviour
    {
        [SerializeField] private GameObject fireEffect;
        private MeshRenderer meshRenderer;
        [HideInInspector] public new Collider collider;
        [HideInInspector] public new Rigidbody rigidbody;

        private int destroyTimer;
        private const int TimeToLiveAfterThrow = 100;

        public bool IsBurned { get; private set; }
        public bool IsBurning { get; private set; }
        public bool IsCooked { get; private set; }
        private bool isOvercooking;

        private static readonly Color EatableCookedColor = new Color(0.961f, 0.837f, 0.680f);
        private static readonly Color PerfectlyCookedColor = new Color(0.9339623f, 0.7260283f, 0.4625756f);
        private static readonly Color FlammableCookedColor = new Color(0.2f, 0.2f, 0.2f);
        private static readonly Color BurnedMaterialColor = Color.black;
        private const float CookingSpeed = 0.01f;

        private static bool AreColorsEqual(Color currentColor, Color eatableColor)
        {
            return Mathf.Abs(currentColor.r - eatableColor.r) < 0.01f &&
                   Mathf.Abs(currentColor.g - eatableColor.g) < 0.01f &&
                   Mathf.Abs(currentColor.b - eatableColor.b) < 0.01f;
        }

        private static bool IsFirstColorLessSecond(Color fistColor, Color secondColor)
        {
            return fistColor.r < secondColor.r &&
                   fistColor.g < secondColor.g &&
                   fistColor.b < secondColor.b;
        }

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            if (destroyTimer == 0)
            {
                return;
            }

            destroyTimer--;
            if (destroyTimer <= 0 && gameObject)
            {
                Destroy(gameObject);
            }
        }

        public void Cook()
        {
            if (IsBurned)
            {
                return;
            }

            if (!isOvercooking)
            {
                CookUntilPerfect();
            }
            else
            {
                CookUntilBurned();
            }
        }

        private void CookUntilPerfect()
        {
            meshRenderer.material.color += (PerfectlyCookedColor - meshRenderer.material.color) * CookingSpeed;

            if (IsFirstColorLessSecond(meshRenderer.material.color, EatableCookedColor))
            {
                IsCooked = true;
            }

            if (AreColorsEqual(meshRenderer.material.color, PerfectlyCookedColor))
            {
                isOvercooking = true;
            }
        }

        private void CookUntilBurned()
        {
            meshRenderer.material.color += (BurnedMaterialColor - meshRenderer.material.color) * CookingSpeed;

            if (IsFirstColorLessSecond(meshRenderer.material.color, FlammableCookedColor))
            {
                Burn();
            }
        }

        public void Burn()
        {
            meshRenderer.material.color = BurnedMaterialColor;
            fireEffect.SetActive(true);
            IsBurned = true;
            IsBurning = true;
        }

        public void Extinguish()
        {
            if (!IsBurned)
            {
                return;
            }

            fireEffect.SetActive(false);
            IsBurning = false;
        }

        public void InitiateSelfDestruction()
        {
            destroyTimer = TimeToLiveAfterThrow;
        }
    }
}
