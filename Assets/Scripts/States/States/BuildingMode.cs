using UnityEngine;

namespace DistilledGames.States
{
    public class BuildingMode : BaseState
    {
        private float timeEntered;

        public override void StateEnter()
        {
           
            base.StateEnter();
            timeEntered = Time.time;


            if (gameManager.PrevState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
                return;

            BuildingManager.instance.Running = false;
            BuildingMenu.instance.SwitchPanel(BuildingMenu.BuildingMenuPanels.BuildingOptions);
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

