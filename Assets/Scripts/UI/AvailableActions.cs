using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerTools.SpaceShipParts;
using StaticObjects;
using TMPro;
using UI.AvailableActionHelpers;
using UnityEngine;

namespace UI
{
    public class AvailableActions : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private TextMeshProUGUI centerActionsTextMesh;
        [SerializeField] private TextMeshProUGUI topRightActionsTextMesh;
        [SerializeField] private GameObject crosshair;

        private readonly List<UiAction> availableActions = new List<UiAction>();

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

            List<UiAction> centerActions = availableActions.Where(action => !action.TopRightInsteadOfCenter).ToList();
            List<UiAction> topRightActions = availableActions.Where(action => action.TopRightInsteadOfCenter).ToList();

            centerActionsTextMesh.text = CreateSuggestedActionsText(centerActions);
            topRightActionsTextMesh.text = CreateSuggestedActionsText(topRightActions);
            crosshair.SetActive(centerActions.Count == 0);
        }

        private void AddRaycastActions()
        {
            Transform cachedPlayerCameraTransform = player.camera.transform;
            const float interactDistance = 2f;

            RaycastHit[] raycasts = UnityEngine.Physics.RaycastAll(cachedPlayerCameraTransform.position, cachedPlayerCameraTransform.forward, interactDistance);
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
            if (player.BuckledUppable.IsBuckledUp())
            {
                AddSpaceShipUiActions();
            }

            if (player.MarshmallowRoastable.IsCooking())
            {
                availableActions.Add(new UiAction(KeyCode.F, "Extend stick", null, true));
                availableActions.Add(new UiAction(KeyCode.Q, "Put stick away", () => player.MarshmallowRoastable.StopCooking(), true));
            }
        }

        private void AddSpaceShipUiActions()
        {
            availableActions.Add(new UiAction(KeyCode.Q, "Unbuckle", () => player.BuckledUppable.UnbuckleFromSpaceShipSeat(), true));
            availableActions.Add(new UiAction(KeyCode.F, "Toggle flashlight", () => player.BuckledUppable.ToggleFlashlight(), true));
        }

        private static string CreateSuggestedActionsText(List<UiAction> actions)
        {
            if (actions.Count == 0)
            {
                return "";
            }

            StringBuilder text = new StringBuilder();

            foreach (UiAction action in actions)
            {
                text.Append($"[{action.KeyCode}] - {action.Description}\n");
            }

            return text.ToString();
        }

        private void FireNeededActions()
        {
            foreach (UiAction uiAction in availableActions)
            {
                // Some actions don't need callbacks
                if (uiAction.Callback == null)
                {
                    continue;
                }

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
                    AddSpaceShipHealthAndFuelRefillStationAction(spaceShipHealthAndFuelStation);
                    break;

                case Campfire campfire:
                    AddCampfireRaycastActions(campfire);
                    break;
            }
        }

        private void AddHatchUiActions(SpaceShipHatch spaceShipHatch)
        {
            if (!spaceShipHatch.isClosed)
            {
                return;
            }

            availableActions.Add(new UiAction(KeyCode.E, "open the hatch", spaceShipHatch.Toggle));
        }

        private void AddSpaceShipChairUiAction(SpaceShipSeat spaceShipSeat)
        {
            if (!player.BuckledUppable.IsBuckledUp())
            {
                availableActions.Add(new UiAction(KeyCode.E, "buckle up", () => player.BuckledUppable.BuckleUpIntoSpaceShipSeat(spaceShipSeat)));
            }
        }

        private void AddSpaceShipHealthAndFuelRefillStationAction(SpaceShipHealthAndFuelStation spaceShipHealthAndFuelStation)
        {
            if (!SpaceShipHealthAndFuelStation.CanUseRefill(player))
            {
                return;
            }

            string actionDescription = CreateSpaceShipHealthAndFuelRefillStationActionDescription();

            availableActions.Add(new UiAction(KeyCode.E, actionDescription, () => spaceShipHealthAndFuelStation.ConnectPlayer(player)));
        }

        private void AddCampfireRaycastActions(Campfire campfire)
        {
            if (player.MarshmallowRoastable.IsCooking())
            {
                return;
            }

            availableActions.Add(new UiAction(KeyCode.E, "Roast Marshmallow", () => player.MarshmallowRoastable.StartCooking(campfire)));
        }

        private string CreateSpaceShipHealthAndFuelRefillStationActionDescription()
        {
            List<string> actions = new List<string>();

            if (!player.Damageable.HasFullHealthPoints)
            {
                actions.Add("Use Medkit");
            }

            if (!player.spaceSuit.IsFuelTankFull)
            {
                actions.Add("Refuel Jetpack");
            }

            return string.Join(" and ", actions);
        }
    }
}
