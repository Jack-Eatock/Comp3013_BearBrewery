using UnityEngine;

namespace DistilledGames.States
{
	public class InMenu : BaseState
	{
		public override void StateEnter()
		{
			base.StateEnter();
		}

		public override void StateExit()
		{
			base.StateExit();
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
			return StateDefinitions.ChangeInState.NoChange;
		}

		public override StateDefinitions.ChangeInState EnterBuildMode()
		{
			return StateDefinitions.ChangeInState.NoChange;
		}

		public override StateDefinitions.ChangeInState Escape()
		{
			return StateDefinitions.ChangeInState.NoChange;
		}
	}
}

