using System;
using UnityEngine;
using DistilledGames.States;

namespace DistilledGames
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        protected string activeState, prevState, nextState;
        protected StateDefinitions.IStateManager _activeState, _prevState, _nextState;

        [SerializeField] private BearController bearController;

        #region Getters

        public BearController BearController => bearController; // simplified getter

        public string PrevState
        {
            set { prevState = value; }
            get { return prevState; }
        }

        public string NextState
        {
            set { nextState = value; }
            get { return nextState; }
        }

        public StateDefinitions.IStateManager ActiveState
        {
            get
            {
                return _activeState;
            }
        }

        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                if (this.gameObject != null)
                    Destroy(this.gameObject);
                return;
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
            nextState = StateDefinitions.GameStates.Normal.ToString();
            CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
        }

        private void Update()
        {
            _activeState.StateUpdate();
        }

        #region Handling State

        /// <summary>
        /// Check if we should switch to a different state. 
        /// </summary>
        /// <param name="changeInState"></param>
        public void CheckIfStateShouldChange(StateDefinitions.ChangeInState changeInState)
        {
            if (changeInState == StateDefinitions.ChangeInState.NextState)
                SwitchState(nextState);

            else if (changeInState == StateDefinitions.ChangeInState.PreviousState)
                SwitchState(prevState);
        }

        /// <summary>
        /// Switch to a different state. Exiting last and Starting the desired state.
        /// </summary>
        /// <param name="state">The state to switch to</param>
        public void SwitchState(string state)
        {
            if (_activeState != null)
            {
                _activeState.StateExit();
                prevState = activeState;
            }
            string classDesired = $"DistilledGames.States.{state}";
            Type t = Type.GetType(classDesired);
            StateDefinitions.IStateManager newState = (StateDefinitions.IStateManager)Activator.CreateInstance(t);

            _activeState = newState;
            activeState = state;
            _activeState.StateEnter();
        }

        #endregion
    }

}
