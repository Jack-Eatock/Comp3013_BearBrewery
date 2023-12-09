using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DistilledGames.States
{
    public class BuildingModeDeleting : BaseState
    {
        private Building buildingPlacing;
        private List<Vector3Int> requiredCoords = new List<Vector3Int>();
        private Vector3Int currentSelectedCoords;
        private float timeEntered;
        private bool rotated = false;
        private Direction direction = Direction.Up;
        private RaycastHit2D hit;

        public override void StateEnter()
        {
            base.StateEnter();
            timeEntered = Time.time;

            if (gameManager.PrevState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
                return;

            BuildingManager.instance.Running = false;
            // Camera.main.fieldOfView
            GameManager.Instance.SwitchToCamController(true);
            BuildingManager.instance.ShowGrid(true);
            BuildingManager.instance.ShowArrows(true);
            gameManager.SetBearActive(false);
            gameManager.SetItemsActive(false);
            BuildingMenu.instance.ShowMenu();
        }

        public override void StateExit()
        {
            base.StateExit();

            if (gameManager.NextState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
                return;

            BuildingManager.instance.Running = true;
            GameManager.Instance.SwitchToCamController(false);
            BuildingManager.instance.ShowGrid(false);
            BuildingManager.instance.ShowArrows(false);
            gameManager.SetBearActive(true);
            gameManager.SetItemsActive(true);
            MenuManager.Instance.HideCurrentMenu(MenuManager.Menus.BuildingMenu);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override StateDefinitions.ChangeInState PrimaryInteractionPressed()
        {
            // Check if they clicked on a building.
            hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(PlayerInputHandler.Instance.PrimaryCursorPosition));
            if (!hit.collider)
                return StateDefinitions.ChangeInState.NoChange;

            Debug.Log(hit.collider.transform.name);
            Building building = hit.collider.transform.root.GetComponentInChildren<Building>();

            if (building != null)
            {
                GameManager.Instance.EarnedCash(building.data.Cost);
                BuildingMenu.instance.RefreshCosts();
                BuildingManager.instance.DeleteObject(building);
                AudioManager.instance.SFX_PlayClip("Destroy", 1f);
            }
               

            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState MovementInput(Vector2 input)
        {
            GameManager.Instance.CamController.OnMove(input);
            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState EnterBuildMode()
        {
            if (Time.time - timeEntered <= .5f)
                return StateDefinitions.ChangeInState.NoChange;

            gameManager.NextState = StateDefinitions.GameStates.Normal.ToString();
            return StateDefinitions.ChangeInState.NextState;
        }
    }
}

