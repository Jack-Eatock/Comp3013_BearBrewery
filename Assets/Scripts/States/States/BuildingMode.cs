using UnityEngine;

namespace DistilledGames.States
{
    public class BuildingMode : BaseState
    {
        private float timeEntered;

        public override void StateEnter()
        {
            BuildingManager.instance.Running = false;
            base.StateEnter();
            timeEntered = Time.time;
            BuildingMenu.instance.SwitchPanel(BuildingMenu.BuildingMenuPanels.BuildingOptions);

            if (gameManager.PrevState == StateDefinitions.GameStates.BuildingModePlacing.ToString())
                return;

            GameManager.Instance.SwitchToCamController(true);
            BuildingManager.instance.ShowGrid(true);
            BuildingManager.instance.ShowArrows(true);
            gameManager.SetBearActive(false);
            gameManager.SetItemsActive(false);
            MenuManager.Instance.ShowMenu(MenuManager.Menus.BuildingMenu);
        }

        public override void StateExit()
        {
            BuildingManager.instance.Running = true;
            base.StateExit();

            if (gameManager.NextState == StateDefinitions.GameStates.BuildingModePlacing.ToString())
                return;

            GameManager.Instance.SwitchToCamController(false);
            BuildingManager.instance.ShowGrid(false); 
            BuildingManager.instance.ShowArrows(false);
            gameManager.SetBearActive(true);
            gameManager.SetItemsActive(true);
            MenuManager.Instance.HideMenu(MenuManager.Menus.BuildingMenu);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override StateDefinitions.ChangeInState PrimaryInteractionPressed()
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState SecondaryInteractionPressed()
        {
            // Check if they clicked on a building.
            RaycastHit2D hit;
            hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(PlayerInputHandler.Instance.PrimaryCursorPosition));
            if (!hit.collider)
                return StateDefinitions.ChangeInState.NoChange;

            Debug.Log(hit.collider.transform.name);
            Building building = hit.collider.transform.root.GetComponentInChildren<Building>();

            if (building != null)
                BuildingManager.instance.DeleteObject(building);

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

