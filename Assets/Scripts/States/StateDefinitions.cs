using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

namespace DistilledGames
{
    public class StateDefinitions : MonoBehaviour
    {
        public enum GameStates { Normal, BuildingMode, InMenu }
        public enum ChangeInState { NoChange, NextState, PreviousState }

        public interface IStateManager
        {
            void StateEnter();

            void StateExit();

            void StateUpdate();

            ChangeInState PrimaryInteractionPressed();

            ChangeInState EnterBuildMode();

            ChangeInState MovementInput(Vector2 input);
            ChangeInState SprintInput(bool sprint);
        }
    }
}
