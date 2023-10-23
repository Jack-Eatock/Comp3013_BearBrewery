using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

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

            gameManager.BearController.OnMove(input);

            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState SprintInput(bool sprint)
        {
            Debug.Log("Sprint input" + sprint);

            gameManager.BearController.Moving(sprint);

            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState EnterBuildMode()
        {
            gameManager.NextState = StateDefinitions.GameStates.BuildingMode.ToString();
            return StateDefinitions.ChangeInState.NextState;
        }
    }
}

