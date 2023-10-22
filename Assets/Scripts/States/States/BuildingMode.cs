using UnityEngine;

namespace DistilledGames.States
{
    public class BuildingMode : BaseState
    {
        public override void StateEnter()
        {
            base.StateExit();
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
    }
}

