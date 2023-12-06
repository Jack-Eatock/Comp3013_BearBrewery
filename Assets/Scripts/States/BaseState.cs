using UnityEngine;

namespace DistilledGames.States
{
    public class BaseState : StateDefinitions.IStateManager
    {
        private bool debugging = true;
        protected GameManager gameManager;

        public virtual void StateEnter()
        {
            gameManager = GameManager.Instance;
            if (debugging)
                Debug.Log("Entered state " + this);
        }

        public virtual void StateExit()
        {
            if (debugging)
                Debug.Log("Exited state " + this);
        }

        public virtual void StateUpdate()
        {

        }

        public virtual StateDefinitions.ChangeInState PrimaryInteractionPressed()
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public virtual StateDefinitions.ChangeInState SecondaryInteractionPressed()
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public virtual StateDefinitions.ChangeInState MovementInput(Vector2 input)
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public virtual StateDefinitions.ChangeInState EnterBuildMode()
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public virtual StateDefinitions.ChangeInState RotateInput(int dir)
        {
            return StateDefinitions.ChangeInState.NoChange;
        }
    }
}
