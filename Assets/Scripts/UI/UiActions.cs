using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerLogic;
using PlayerTools.SpaceShipParts;
using StaticObjects;
using TMPro;
using UI.UiActionParts;
using UnityEngine;

namespace UI
{
    /**
     * Ui element that shows the player actions that he can interact with
     */
    public class UiActions : MonoBehaviour
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

            if (player.marshmallowCookable.IsCooking())
            {
                AddMarshmallowCookingActions();
            }
        }

        private void AddMarshmallowCookingActions()
        {
            availableActions.Add(new UiAction(KeyCode.F, "Extend", null, true));
            availableActions.Add(new UiAction(KeyCode.Q, "Put away", () => player.marshmallowCookable.StopCooking(), true));

            if (player.playerControllable.extendMarshmallowStick)
            {
                return;
            }

            if (player.marshmallowCookable.marshmallow == null)
            {
                availableActions.Add(new UiAction(KeyCode.E, "Replace", () => player.marshmallowCookable.ReplaceMarshmallow()));
            }
            else if (player.marshmallowCookable.marshmallow.IsBurning)
            {
                availableActions.Add(new UiAction(KeyCode.E, "Extinguish", () => player.marshmallowCookable.marshmallow.Extinguish()));
            }
            else
            {
                if (player.marshmallowCookable.marshmallow.IsCooked || player.marshmallowCookable.marshmallow.IsBurned)
                {
                    availableActions.Add(new UiAction(KeyCode.E, "Eat", () => player.marshmallowCookable.EatMarshmallow()));
                }

                if (player.marshmallowCookable.marshmallow.IsBurned)
                {
                    availableActions.Add(new UiAction(KeyCode.R, "Toss", () => player.marshmallowCookable.ThrowBurnedMarshmallow()));
                }
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
            if (player.marshmallowCookable.IsCooking())
            {
                return;
            }

            availableActions.Add(new UiAction(KeyCode.E, "Roast Marshmallow", () => player.marshmallowCookable.StartCooking(campfire)));
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
