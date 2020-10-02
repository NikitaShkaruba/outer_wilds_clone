using System.Collections.Generic;
using System.Text;
using PlayerTools.SpaceShipParts;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Player))]
    public class UserInterface : MonoBehaviour
    {
        private Player player;
        [SerializeField] private TextMeshProUGUI suggestedActionsTextMeshPro;

        private readonly List<UiAction> availableActions = new List<UiAction>();

        private void Start()
        {
            player = GetComponent<Player>();
        }

        private void Update()
        {
            ShowAvailableActions();
            FireNeededActions();
        }

        private void ShowAvailableActions()
        {
            availableActions.RemoveAll(action => true);

            AddRaycastActions();
            AddStateActions();

            suggestedActionsTextMeshPro.text = CreateSuggestedActionsText();
        }

        private void AddRaycastActions()
        {
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
        }

        private void AddStateActions()
        {
            if (player.isBuckledUp || player.buckleUpTransitionGoing)
            {
                AddSpaceShipUiActions();
            }
        }

        private void AddSpaceShipUiActions()
        {
            availableActions.Add(new UiAction(KeyCode.Q, "Unbuckle", () => player.Unbuckle()));
            availableActions.Add(new UiAction(KeyCode.F, "Toggle flashlight", () => player.pilotedSpaceShip.ToggleFlashlight()));
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
            switch (monoBehaviour)
            {
                case SpaceShipHatch hatch:
                    AddHatchUiActions(hatch);
                    break;

                case SpaceShipSeat spaceShipSeat:
                    AddSpaceShipChairUiAction(spaceShipSeat);
                    break;

                case SpaceShipHealthAndFuelStation spaceShipHealthAndFuelStation:
                    AddSpaceShipHealthAndFuelRefillStation(spaceShipHealthAndFuelStation);
                    break;
            }
        }

        private void AddHatchUiActions(SpaceShipHatch spaceShipHatch)
        {
            if (!spaceShipHatch.IsClosed)
            {
                return;
            }

            availableActions.Add(new UiAction(KeyCode.E, "open the hatch", spaceShipHatch.Open));
        }

        private void AddSpaceShipChairUiAction(SpaceShipSeat spaceShipSeat)
        {
            if (!player.isBuckledUp && !player.buckleUpTransitionGoing)
            {
                availableActions.Add(new UiAction(KeyCode.E, "buckle up", () => player.StartBucklingUp(spaceShipSeat)));
            }
        }

        private void AddSpaceShipHealthAndFuelRefillStation(SpaceShipHealthAndFuelStation spaceShipHealthAndFuelStation)
        {
            if (!SpaceShipHealthAndFuelStation.CanUseRefill(player))
            {
                return;
            }

            string actionDescription = CreateSpaceShipHealthAndFuelRefillStationActionDescription();

            availableActions.Add(new UiAction(KeyCode.E, actionDescription, () => spaceShipHealthAndFuelStation.ConnectPlayer(player)));
        }

        private string CreateSpaceShipHealthAndFuelRefillStationActionDescription()
        {
            List<string> actions = new List<string>();

            if (!player.Damageable.HasFullHealthPoints)
            {
                actions.Add("Use Medkit");
            }

            if (!player.SpaceSuit.IsFuelTankFull)
            {
                actions.Add("Refuel Jetpack");
            }

            return string.Join(" and ", actions);
        }
    }
}
