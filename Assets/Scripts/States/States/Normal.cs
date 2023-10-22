using UnityEngine;

namespace DistilledGames.States
{
    public class Normal : BaseState
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
            Debug.Log("Movement input" + input);


            /// Add movement here!

            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState EnterBuildMode()
        {
            gameManager.NextState = StateDefinitions.GameStates.BuildingMode.ToString();
            return StateDefinitions.ChangeInState.NextState;
        }
    }
}

