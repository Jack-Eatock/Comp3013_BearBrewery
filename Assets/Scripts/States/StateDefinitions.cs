using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

namespace DistilledGames
{
    public class StateDefinitions : MonoBehaviour
    {
        public enum GameStates { Normal, BuildingMode, InMenu, BuildingModePlacing, EndOfDay, BuildingModeDeleting }
        public enum ChangeInState { NoChange, NextState, PreviousState }

        public interface IStateManager
        {
            void StateEnter();

            void StateExit();

            void StateUpdate();

            ChangeInState PrimaryInteractionPressed();

            ChangeInState SecondaryInteractionPressed();

            ChangeInState RotateInput(int dir);

            ChangeInState EnterBuildMode();

            ChangeInState MovementInput(Vector2 input);
        }
    }
}
