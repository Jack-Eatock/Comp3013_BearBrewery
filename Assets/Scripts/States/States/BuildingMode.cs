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
			{
				return;
			}

			BuildingManager.Instance.Running = false;
			BuildingMenu.Instance.SwitchPanel(BuildingMenu.BuildingMenuPanels.BuildingOptions);
			GameManager.Instance.SwitchToCamController(true);
			BuildingManager.Instance.ShowGrid(true);
			BuildingManager.Instance.ShowArrows(true);
			gameManager.SetBearActive(false);
			gameManager.SetItemsActive(false);
			BuildingMenu.Instance.ShowMenu();
		}

		public override void StateExit()
		{

			base.StateExit();

			if (gameManager.NextState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
			{
				return;
			}

			BuildingManager.Instance.Running = true;
			GameManager.Instance.SwitchToCamController(false);
			BuildingManager.Instance.ShowGrid(false);
			BuildingManager.Instance.ShowArrows(false);
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
			{
				return StateDefinitions.ChangeInState.NoChange;
			}

			gameManager.NextState = StateDefinitions.GameStates.Normal.ToString();
			return StateDefinitions.ChangeInState.NextState;
		}
	}
}

