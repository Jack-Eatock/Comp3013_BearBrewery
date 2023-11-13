using System;
using UnityEngine;
using DistilledGames.States;
using System.Collections.Generic;
using Cinemachine;

namespace DistilledGames
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] protected string activeState, prevState, nextState;
        protected StateDefinitions.IStateManager _activeState, _prevState, _nextState;

        private BearController bearController;
        private CamController camController;
        [SerializeField]
        private CinemachineVirtualCamera brainController;

        [SerializeField] private GameConfig gameConfig;

        // Playthrough
        private List<Item> items = new List<Item>();
        private double cash;

        // Settings
        public float ConveyerBeltsTimeToMove = 1;

        #region Getters

        public BearController BearController => bearController; // simplified getter
        public CamController CamController => camController;
        public GameConfig GameConfig => gameConfig;

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
            bearController = GameObject.FindGameObjectWithTag("Bear").GetComponent<BearController>();
            camController = GameObject.FindGameObjectWithTag("Cam").GetComponent<CamController>();
            nextState = StateDefinitions.GameStates.Normal.ToString();
            CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
        }

        private void Update()
        {
            _activeState.StateUpdate();
        }

        public void SetBearActive(bool state)
        {
            bearController.gameObject.SetActive(state);
        }

        private void ClearItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null || items[i].gameObject == null)
                {
                    items.RemoveAt(i);
                    i = 0;
                }
            }
        }

        public void SwitchToCamController(bool state)
        {
            if (state)
            {
                camController.transform.position = bearController.transform.position;
                brainController.Follow = camController.transform;
            }
            else
                brainController.Follow = bearController.transform;
        }

        public void SetItemsActive(bool state)
        {
            ClearItems();
            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    items[i]?.gameObject.SetActive(state);
                }
                catch (Exception)
                {
                    Debug.Log("ERROR");
                }
            }
        }

        public void RegisterItem(Item item)
        {
            items.Add(item);
        }

        /// <summary>
        /// When an item is destroyed we should de register it.
        /// </summary>
        /// <param name="item"></param>
        public void DeRegisterItem(Item item)
        {
            items.Remove(item); 
        }

        #region Cash

        public void EarnedCash(double _cash)
        {
            cash += _cash;
            UserInterface.Instance.UpdateRevText(cash.ToString());
        }
        #endregion

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
