using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DistilledGames
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }
		[SerializeField] protected string activeState, prevState, nextState;
		protected StateDefinitions.IStateManager _activeState, _prevState, _nextState;
		[SerializeField]
		private CinemachineVirtualCamera brainController;

		[SerializeField] private GameConfig gameConfig;

		[SerializeField]
		private float startingCash = 200;

		// Playthrough
		private readonly List<Item> items = new();

		// Settings
		public float ConveyerBeltsTimeToMove = 1;

		#region Getters
		public double Cash { get; private set; }
		public BearController BearController { get; private set; } // simplified getter
		public CamController CamController { get; private set; }
		public GameConfig GameConfig => gameConfig;

		public string PrevState
		{
			set => prevState = value;
			get => prevState;
		}

		public string NextState
		{
			set => nextState = value;
			get => nextState;
		}

		public StateDefinitions.IStateManager ActiveState => _activeState;

		#endregion

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				if (gameObject != null)
				{
					Destroy(gameObject);
				}

				return;
			}
			else
			{
				Instance = this;
			}
			DontDestroyOnLoad(gameObject);
			BearController = GameObject.FindGameObjectWithTag("Bear").GetComponent<BearController>();
			CamController = GameObject.FindGameObjectWithTag("Cam").GetComponent<CamController>();
			nextState = StateDefinitions.GameStates.Normal.ToString();
			CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
		}

        private void Start()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == "OutsideScene")
            {
                AudioManager.Instance.Music_PlayTrack("Outdoors");
            }
            else
            {
                AudioManager.Instance.Music_PlayTrack("Music");
            }
        }

        public void StartGame()
		{
			MenuManager.Instance.SetGUIState(true);
			Cash = startingCash;
			UserInterface.Instance.UpdateRevText(Cash.ToString());

			nextState = StateDefinitions.GameStates.Normal.ToString();
			CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
		}

		private void Update()
		{
			_activeState.StateUpdate();
		}

		public void SetBearActive(bool state)
		{
			BearController.gameObject.SetActive(state);
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
				CamController.transform.position = BearController.transform.position;
			}
			else
			{
			}

			CameraManager.Instance.SetBuildMode(state);
		}

		public void SetItemsActive(bool state)
		{
			ClearItems();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					if (items[i] != null)
						items[i].gameObject.SetActive(state);
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
			_ = items.Remove(item);
		}

		#region Cash

		public void EarnedCash(double _cash)
		{
			AudioManager.Instance.SFX_PlayClip("MadeCash", 1f);
			Cash += _cash;
			UserInterface.Instance.UpdateRevText(Cash.ToString());
		}

		public void SpentCash(double _cash)
		{
			Cash -= _cash;
			UserInterface.Instance.UpdateRevText(Cash.ToString());
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
			{
				SwitchState(nextState);
			}
			else if (changeInState == StateDefinitions.ChangeInState.PreviousState)
			{
				SwitchState(prevState);
			}
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
