using DistilledGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using UnityEngine.Windows;

public class BearController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // multiplier for sprinting
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private Image circleImage;

    private bool isItemHeld = false;
    private Item heldItem;
    private int heldItemOriginalSortingOrder;

    private bool isSprinting = false; // Track if the bear is currently sprinting
    private Vector3 currentMoveDirection = Vector3.zero;

    private float interactHoldDuration = 0.5f; // the duration after which the button press is considered a hold
    private float timeSinceInteractPressed = 0f;
    private bool interactPressed = false;
    private bool interactHoldTriggered = false;

    public bool IsItemHeld => isItemHeld;

    void Update()
    {
        //Get Sprint state directly from PlayerInputHandler
        isSprinting = PlayerInputHandler.Instance.Sprint;

        // Checking for InteractHold
        if (PlayerInputHandler.Instance.Interact && !interactPressed)
        {
            interactPressed = true;
            interactHoldTriggered = false; // Reset this flag on a new button press
            timeSinceInteractPressed = 0f;
        }
        else if (!PlayerInputHandler.Instance.Interact && interactPressed)
        {
            interactPressed = false;
            if (timeSinceInteractPressed < interactHoldDuration)
            {
                Debug.Log("Interacting");
                Interacting();  // If the button was released before it's considered a hold
            }
        }

        if (interactPressed)
        {
            timeSinceInteractPressed += Time.deltaTime;

            if (timeSinceInteractPressed >= interactHoldDuration && !interactHoldTriggered)
            {
                Debug.Log("Interacting Held");
                InteractHold(); // Handle hold interaction
                interactHoldTriggered = true; // Set to true to avoid triggering the hold multiple times
            }
        }

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed; // Adjust the speed based on sprinting state
        transform.position += currentSpeed * Time.deltaTime * currentMoveDirection;
    }

    public void OnMove(Vector2 input)
    {
        currentMoveDirection = new Vector3(input.x, input.y, 0);
    }

    public void Interacting()
    {
        if (heldItem != null)
        {
            // Drop the held item slightly below the center of the collider
            heldItem.transform.position = detectionCollider.transform.position + new Vector3(0, -detectionCollider.radius * 0.2f, 0);
            heldItem.transform.parent = null; // Unparent the item so it doesn't move with the bear
            heldItem.GetComponent<SpriteRenderer>().sortingOrder = heldItemOriginalSortingOrder; // Reset the sorting order
            heldItem = null; // Reset the held item reference
            isItemHeld = false;
        }
        else
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCollider.transform.position, detectionCollider.radius);

            foreach (var hitCollider in hitColliders)
            {
                Item item = hitCollider.GetComponent<Item>();

                if (item != null && item.IsInteractable)
                {
                    // Pick up the interactable item and move it to the center of the collider
                    item.transform.position = detectionCollider.transform.position;
                    item.transform.parent = detectionCollider.transform; // Parent the item to the collider so it moves with the bear
                    heldItemOriginalSortingOrder = item.GetComponent<SpriteRenderer>().sortingOrder; // Store original sorting order
                    item.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1; // Ensure item is behind the bear
                    heldItem = item; // Set the held item reference
                    Debug.Log("Picked up an interactable item!");
                    isItemHeld = true;
                    break; // Bear can hold only one item, so we break once we've found one
                }
            }
        }
    }


    public void InteractHold()
    {
        if (isItemHeld)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCollider.transform.position, detectionCollider.radius);
            foreach (var hitCollider in hitColliders)
            {
                DepositBox depositBox = hitCollider.GetComponent<DepositBox>();
                if (depositBox != null)
                {
                    // Deposit item to the box and check its value
                    depositBox.DepositItem(heldItem);
                    heldItem = null;
                    isItemHeld = false;
                    break;
                }
            }
        }
    }
}