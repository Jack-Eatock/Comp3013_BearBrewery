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

    private Item heldItem;
    private int heldItemOriginalSortingOrder;

    private bool isSprinting = false; // Track if the bear is currently sprinting
    private Vector3 currentMoveDirection = Vector3.zero;

    void Update()
    {
        // Fetch sprint state directly from PlayerInputHandler
        isSprinting = PlayerInputHandler.Instance.Sprint;

        // If you want the bear to perform some interaction when the Interact property is true
        if (PlayerInputHandler.Instance.Interact)
        {
            Interacting();
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
                    break; // Bear can hold only one item, so we break once we've found one
                }
            }
        }
    }


}

