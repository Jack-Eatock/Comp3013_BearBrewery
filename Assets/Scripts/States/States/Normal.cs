using UnityEngine;

namespace DistilledGames.States
{
	public class Normal : BaseState
	{
		private float timeOnEnter;
		public override void StateEnter()
		{
			base.StateEnter();
			timeOnEnter = Time.time;
		}

		public override void StateExit()
		{
			base.StateExit();
			_ = MovementInput(Vector3.zero); // reset movement values;
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
			Debug.Log("Movement input" + input);

			gameManager.BearController.OnMove(input);

			return StateDefinitions.ChangeInState.NoChange;
		}

		public override StateDefinitions.ChangeInState EnterBuildMode()
		{
			if (Time.time - timeOnEnter <= .5f)
			{
				return StateDefinitions.ChangeInState.NoChange;
			}

			gameManager.NextState = StateDefinitions.GameStates.BuildingMode.ToString();
			return StateDefinitions.ChangeInState.NextState;
		}

		public override StateDefinitions.ChangeInState Escape()
		{
			MainMenu.Instance.ShowMenu();
			MainMenu.Instance.SetupMenu(true);
			return StateDefinitions.ChangeInState.NoChange;
		}
	}
}

