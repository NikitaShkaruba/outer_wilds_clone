using System.Collections.Generic;
using System.Text;
using TMPro;
using Tools.SpaceShipParts;
using UnityEngine;

namespace UI
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private TextMeshProUGUI suggestedActionsTextMeshPro;

        private readonly List<UiAction> availableActions = new List<UiAction>();

        private void Update()
        {
            ShowAvailableActions();
            FireNeededActions();
        }

        private void ShowAvailableActions()
        {
            availableActions.RemoveAll(action => true);

            Transform cachedPlayerCameraTransform = player.camera.transform;
            const float interactDistance = 2f;

            RaycastHit[] raycasts = Physics.RaycastAll(cachedPlayerCameraTransform.position, cachedPlayerCameraTransform.forward, interactDistance);
            if (raycasts.Length == 0)
            {
                return;
            }

            foreach (RaycastHit raycastHit in raycasts)
            {
                MonoBehaviour[] hitMonoBehaviours = raycastHit.collider.gameObject.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in hitMonoBehaviours)
                {
                    AddUiActions(monoBehaviour);
                }
            }

            suggestedActionsTextMeshPro.text = CreateSuggestedActionsText();
        }

        private string CreateSuggestedActionsText()
        {
            if (availableActions.Count == 0)
            {
                return "";
            }

            StringBuilder text = new StringBuilder();

            foreach (UiAction action in availableActions)
            {
                text.Append($"[{action.KeyCode}] - {action.Description}\n");
            }

            return text.ToString();
        }

        private void FireNeededActions()
        {
            foreach (UiAction uiAction in availableActions)
            {
                bool isButtonPressed = Input.GetKeyDown(uiAction.KeyCode);
                if (isButtonPressed)
                {
                    uiAction.Callback();
                }
            }
        }

        private void AddUiActions(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is Hatch hatch)
            {
                AddHatchUiActions(hatch);
            }
        }

        private void AddHatchUiActions(Hatch hatch)
        {
            if (!hatch.IsClosed)
            {
                return;
            }

            availableActions.Add(new UiAction(KeyCode.E, "open the hatch", hatch.Open));
        }
    }
}