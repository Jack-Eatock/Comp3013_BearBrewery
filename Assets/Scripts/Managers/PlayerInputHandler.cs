using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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

        private Vector2 primaryCursorPosition;
        private Vector2 movementInput;
        private bool sprint;
        private bool interact;

        // Scroll speed
        private float timeOfLastScroll = 0;
        private float timeBetweenScroll = .15f;
        

        #region Getters

        public bool Sprint => sprint;
        public bool Interact => interact;
        public Vector2 PrimaryCursorPosition => primaryCursorPosition;

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
                Instance = this;

            DontDestroyOnLoad(this.gameObject);
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
        }

        private void Update()
        {   
            if (playerSprint.WasPressedThisFrame())
            {
                sprint = true;
            }
            if (playerSprint.WasReleasedThisFrame())
            {
                sprint = false;
            }

            if (playerInteract.WasPressedThisFrame())
            {
                interact = true;
            }
            if (playerInteract.WasReleasedThisFrame())
            {
                interact = false;
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
            int dir = 0;
            if (x > 0)
                dir = 1;
            else
                dir = -1;

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

