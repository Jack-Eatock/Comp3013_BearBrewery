using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames.States
{
	public class BuildingModeDeleting : BaseState
	{
		private float timeEntered;
		private RaycastHit2D hit;

		public override void StateEnter()
		{
			base.StateEnter();
			timeEntered = Time.time;

			if (gameManager.PrevState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
			{
				return;
			}

			BuildingManager.Instance.Running = false;
			// Camera.main.fieldOfView
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
			// Check if they clicked on a building.
			hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(PlayerInputHandler.Instance.PrimaryCursorPosition));
			if (!hit.collider)
			{
				return StateDefinitions.ChangeInState.NoChange;
			}

			Debug.Log(hit.collider.transform.name);
			Building building = hit.collider.transform.root.GetComponentInChildren<Building>();

			if (building != null)
			{
				GameManager.Instance.EarnedCash(building.Data.Cost);
				BuildingMenu.Instance.RefreshCosts();
				BuildingManager.Instance.DeleteObject(building);
				AudioManager.Instance.SFX_PlayClip("Destroy", 1f);
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
			{
				return StateDefinitions.ChangeInState.NoChange;
			}

			gameManager.NextState = StateDefinitions.GameStates.Normal.ToString();
			return StateDefinitions.ChangeInState.NextState;
		}
	}
}

