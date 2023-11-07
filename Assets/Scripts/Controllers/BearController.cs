using DistilledGames;
using UnityEngine;

public class BearController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // multiplier for sprinting
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float timeBetweenInteractions = .4f;
    [SerializeField] private Vector3 interactionZoneOffsetLeft = new Vector3(-0.8f, 0f, 0f);
    [SerializeField] private Vector3 interactionZoneOffsetRight = new Vector3(0.8f, 0f, 0f);
    [SerializeField] private Transform interactionZoneTransform;

    private Rigidbody2D rig;
    private SpriteRenderer rend;

    private bool isItemHeld = false;
    private Item heldItem;
    private int heldItemOriginalSortingOrder;
    private Vector3 currentMoveDirection = Vector3.zero;
    private float timeLastInteracted = 0;
    private float lastNonZeroInputX = -1;  // Initialize to -1 assuming default facing left

    public bool IsItemHeld => isItemHeld;
    public Item HeldItem => heldItem;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        if (interactionZoneTransform == null)
        {
            Debug.LogError("Interaction Zone Transform is not assigned in the inspector.", this);
        }
    }

    void Update()
    {
       if (PlayerInputHandler.Instance.Interact)
            Interact();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Helper.UpdateSortingOrder(rend, transform);
        if (heldItem != null)
            Helper.UpdateSortingOrder(heldItem.Rend, heldItem.transform, 1);

        FlipSpriteBasedOnMoveDirection();
    }

    #region Movement

    private void MovePlayer()
    {
        // Move using physics to allow collisions etc.
        float currentSpeed = PlayerInputHandler.Instance.Sprint ? moveSpeed * sprintMultiplier : moveSpeed; // Adjust the speed based on sprinting state
        rig.MovePosition(transform.position + (currentSpeed * Time.deltaTime * currentMoveDirection));
    }

    public void OnMove(Vector2 input)
    {
        currentMoveDirection = new Vector3(input.x, input.y, 0);
    }

    private void FlipSpriteBasedOnMoveDirection()
    {
        // Check if there is horizontal movement
        if (currentMoveDirection.x != 0)
        {
            // If moving right and sprite facing left or moving left and sprite facing right, flip the sprite.
            if ((currentMoveDirection.x > 0 && !rend.flipX) || (currentMoveDirection.x < 0 && rend.flipX))
            {
                rend.flipX = !rend.flipX; // This flips the sprite
                // Also move the interaction zone to the correct position
                interactionZoneTransform.localPosition = rend.flipX ? interactionZoneOffsetRight : interactionZoneOffsetLeft;
            }

            // Update the lastNonZeroInputX
            lastNonZeroInputX = currentMoveDirection.x;
        }
        else if (lastNonZeroInputX != 0) // No current horizontal movement, use last known direction
        {
            // If last input was right and sprite facing left or last input was left and sprite facing right, flip the sprite.
            if ((lastNonZeroInputX > 0 && !rend.flipX) || (lastNonZeroInputX < 0 && rend.flipX))
            {
                rend.flipX = !rend.flipX; // This flips the sprite
                // Also move the interaction zone to the correct position
                interactionZoneTransform.localPosition = rend.flipX ? interactionZoneOffsetRight : interactionZoneOffsetLeft;
            }
        }
    }

    #endregion

    #region Interactions

    public void Interact()
    {
        if (Time.time - timeLastInteracted < timeBetweenInteractions)
            return;

        timeLastInteracted = Time.time;
        Debug.Log("Interacting");

        // The detection collider is on the interaction layer so it will only collide with other interactions.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCollider.transform.position, detectionCollider.radius, interactionLayer);
        if (heldItem != null)
        {
            if (TryInsertItem(hitColliders)) {}
            else
                DropHeldItem();
        }
        else
            TryPickUpItem(hitColliders);
    }

    private bool TryInsertItem(Collider2D[] hitColliders)
    {
        IInteractable interactable;
        bool foundInteractable = false;
        foreach (var hitCollider in hitColliders)
        {
            // ignore the item being held.
            if (hitCollider.transform.parent == heldItem.transform)
                continue;

            if (hitCollider.transform.parent.TryGetComponent(out interactable))
            {
                foundInteractable = true;
                // Found an interactable object
                // Try inserting it. If it works great, otherwise keep checking.
                if (interactable.TryToInsertItem(heldItem))
                {
                    heldItem = null;
                    return true;
                }
            }
        }
        // Even if we cant interact we dont want them to drop an ingredient when trying to interact with a building. So do nothing.
        return foundInteractable;
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
        IInteractable interactable;
        Item itemRetrieved = null;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.parent.TryGetComponent(out interactable))
            {
                // Found an interactable item.
                // Try to retrieve item. If not possible keep checking other things.
                if (interactable.TryToRetreiveItem(out itemRetrieved))
                {
                    PickUpItem(itemRetrieved);
                    break;
                }
            }
        }
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

    #endregion
}