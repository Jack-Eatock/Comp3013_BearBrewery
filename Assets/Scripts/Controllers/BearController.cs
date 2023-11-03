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
    private Rigidbody2D rig;
    private SpriteRenderer rend;
    [SerializeField] private Image circleImage;

    private bool isItemHeld = false;
    private Item heldItem;
    private int heldItemOriginalSortingOrder;

    private Vector3 currentMoveDirection = Vector3.zero;

    private float interactHoldDuration = 0.5f; // the duration after which the button press is considered a hold
    private float timeSinceInteractPressed = 0f;
    private bool interactPressed = false;
    private bool interactHoldTriggered = false;

    public bool IsItemHeld => isItemHeld;
    public Item HeldItem => heldItem;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
    }

    private void FixedUpdate()
    {
        float currentSpeed = PlayerInputHandler.Instance.Sprint ? moveSpeed * sprintMultiplier : moveSpeed; // Adjust the speed based on sprinting state
        //transform.position += currentSpeed * Time.deltaTime * currentMoveDirection;
        rig.MovePosition(transform.position + (currentSpeed * Time.deltaTime * currentMoveDirection));
        UpdateSortingOrder();
    }

    public void OnMove(Vector2 input)
    {
        currentMoveDirection = new Vector3(input.x, input.y, 0);
    }

    public void Interacting()
    {
        if (heldItem != null)
        {
            DropHeldItem();
        }
        else
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCollider.transform.position, detectionCollider.radius);
            TryPickUpItem(hitColliders);
        }
    }

    private void DropHeldItem()
    {
        // Drop the held item slightly below the center of the collider
        heldItem.transform.position = detectionCollider.transform.position + new Vector3(0, -detectionCollider.radius * 0.2f, 0);
        heldItem.transform.parent = null;
        heldItem.GetComponent<SpriteRenderer>().sortingOrder = heldItemOriginalSortingOrder;
        heldItem = null;
        isItemHeld = false;
    }

    private void TryPickUpItem(Collider2D[] hitColliders)
    {
        foreach (var hitCollider in hitColliders)
        {
            if (TryPickUpFromBoiler(hitCollider)) return;
            if (TryPickUpInteractableItem(hitCollider)) break;
            if (TryPickUpFromDepot(hitCollider)) break;
        }
    }

    private bool TryPickUpFromBoiler(Collider2D hitCollider)
    {
        Boiler boiler = hitCollider.GetComponent<Boiler>();
        if (boiler != null)
        {
            Item outputItem = boiler.RemoveItem();
            if (outputItem != null)
            {
                PickUpItem(outputItem);
                Debug.Log("Picked up an item from the boiler!");
                return true;
            }
        }
        return false;
    }

    private bool TryPickUpInteractableItem(Collider2D hitCollider)
    {
        Item item = hitCollider.GetComponent<Item>();
        if (item != null && item.IsInteractable)
        {
            PickUpItem(item);
            Debug.Log("Picked up an interactable item!");
            return true;
        }
        return false;
    }

    private bool TryPickUpFromDepot(Collider2D hitCollider)
    {
        Depot depot = hitCollider.GetComponent<Depot>();
        if (depot != null)
        {
            Item dispensedItem = depot.DispenseItem();
            if (dispensedItem != null)
            {
                PickUpItem(dispensedItem);
                Debug.Log("Picked up an item from depot!");
                return true;
            }
        }
        return false;
    }

    private void PickUpItem(Item item)
    {
        item.transform.position = detectionCollider.transform.position;
        item.transform.parent = detectionCollider.transform;
        heldItemOriginalSortingOrder = item.GetComponent<SpriteRenderer>().sortingOrder;
        item.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
        heldItem = item;
        isItemHeld = true;
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

                Boiler boiler = hitCollider.GetComponent<Boiler>();
                if (boiler != null)
                {
                    // Add item to the boiler for processing
                    boiler.AddItem(heldItem);
                    heldItem = null;
                    isItemHeld = false;
                    break;
                }
            }
        }
    }

    private void UpdateSortingOrder()
    {
        rend.sortingOrder = 100 - Mathf.RoundToInt(transform.position.y);
    }


}