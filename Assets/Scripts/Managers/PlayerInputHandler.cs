using UnityEngine;
using UnityEngine.InputSystem;

namespace DistilledGames
{
	public class PlayerInputHandler : MonoBehaviour
	{
		public static PlayerInputHandler Instance;
		private PlayerInput playerInput;
		private GameManager gamemanager;

		private InputAction primaryCursorPos;
		private InputAction primaryCursorInteraction;
		private InputAction playerMovement;
		private InputAction playerSprint;
		private InputAction playerInteract;
		private InputAction enterBuildMode;
		private InputAction secondaryCursorInteraction;
		private InputAction rotate;
		private InputAction rotateScroll;
		private InputAction escape;

		private Vector2 primaryCursorPosition;
		private Vector2 movementInput;

		// Scroll speed
		private float timeOfLastScroll = 0;
		private readonly float timeBetweenScroll = .15f;

		#region Getters

		public bool Sprint { get; private set; }
		public bool Interact { get; private set; }
		public Vector2 PrimaryCursorPosition => primaryCursorPosition;

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
			playerInput = GetComponent<PlayerInput>();
			gamemanager = GetComponent<GameManager>();

			primaryCursorPos = playerInput.actions["PrimaryCursorPos"];
			primaryCursorInteraction = playerInput.actions["PrimaryCursorClick"];
			playerMovement = playerInput.actions["Movement"];
			playerSprint = playerInput.actions["Sprint"];
			playerInteract = playerInput.actions["Interaction"];
			enterBuildMode = playerInput.actions["EnterBuildMode"];
			secondaryCursorInteraction = playerInput.actions["SecondaryCursorClick"];
			rotate = playerInput.actions["Rotate"];
			rotateScroll = playerInput.actions["RotateScroll"];
			escape = playerInput.actions["Escape"];
		}

		private void Update()
		{
			if (playerSprint.WasPressedThisFrame())
			{
				Sprint = true;
			}
			if (playerSprint.WasReleasedThisFrame())
			{
				Sprint = false;
			}

			if (playerInteract.WasPressedThisFrame())
			{
				Interact = true;
			}
			if (playerInteract.WasReleasedThisFrame())
			{
				Interact = false;
			}
		}

		private void OnEnable()
		{
			playerInput.controlsChangedEvent.AddListener(OnControlsChangedEvent);
			playerInput.deviceLostEvent.AddListener(OnLostConnectionToDevice);
			playerInput.deviceLostEvent.AddListener(OnRegainedConnectionToDevice);

			playerMovement.performed += InputMovement;
			primaryCursorInteraction.performed += InputPrimaryCursorInteraction;
			primaryCursorPos.performed += InputPrimaryCursorPos;
			enterBuildMode.performed += InputEnterBuildMode;
			secondaryCursorInteraction.performed += InputSecondaryCursorInteraction;
			rotate.performed += InputRotate;
			rotateScroll.performed += InputRotateScroll;
			escape.performed += InputEscape;
		}

		private void OnDisable()
		{
			playerInput.controlsChangedEvent.RemoveListener(OnControlsChangedEvent);
			playerInput.deviceLostEvent.RemoveListener(OnLostConnectionToDevice);
			playerInput.deviceLostEvent.RemoveListener(OnRegainedConnectionToDevice);

			playerMovement.performed -= InputMovement;
			primaryCursorInteraction.performed -= InputPrimaryCursorInteraction;
			primaryCursorPos.performed -= InputPrimaryCursorPos;
			enterBuildMode.performed -= InputEnterBuildMode;
			secondaryCursorInteraction.performed -= InputSecondaryCursorInteraction;
			rotate.performed -= InputRotate;
			escape.performed -= InputEscape;
		}

		private void InputMovement(InputAction.CallbackContext ctx)
		{
			movementInput = ctx.ReadValue<Vector2>();
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.MovementInput(movementInput));
		}

		private void InputPrimaryCursorPos(InputAction.CallbackContext ctx)
		{
			primaryCursorPosition = ctx.ReadValue<Vector2>();
		}

		private void InputEscape(InputAction.CallbackContext ctx)
		{
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.Escape());
		}

		private void InputPrimaryCursorInteraction(InputAction.CallbackContext ctx)
		{
			Debug.Log("CLICK");
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.PrimaryInteractionPressed());
		}

		private void InputEnterBuildMode(InputAction.CallbackContext ctx)
		{
			Debug.Log("Enter Build Mode Pressed");
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.EnterBuildMode());
		}

		private void InputSecondaryCursorInteraction(InputAction.CallbackContext ctx)
		{
			Debug.Log("Secondary CLICK");
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.SecondaryInteractionPressed());
		}

		private void InputRotate(InputAction.CallbackContext ctx)
		{
			gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.RotateInput(1));
		}

		private void InputRotateScroll(InputAction.CallbackContext ctx)
		{
			float x = ctx.ReadValue<float>();
			int dir = x > 0 ? -1 : 1;
			if (Time.time - timeOfLastScroll > timeBetweenScroll)
			{
				gamemanager.CheckIfStateShouldChange(gamemanager.ActiveState.RotateInput(dir));
				timeOfLastScroll = Time.time;
			}
		}

		#region Device events 

		private void OnControlsChangedEvent(PlayerInput obj)
		{
			Debug.Log("Controls scheme changed: " + playerInput.currentControlScheme);
		}

		private void OnLostConnectionToDevice(PlayerInput obj)
		{
			Debug.Log("Device lost connection");
		}

		private void OnRegainedConnectionToDevice(PlayerInput obj)
		{
			Debug.Log("Device regained connection");
		}

		#endregion

	}
}

