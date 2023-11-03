using DistilledGames;
using UnityEngine;

public class BearController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // multiplier for sprinting
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float timeBetweenInteractions = .4f;

    private Rigidbody2D rig;
    private SpriteRenderer rend;

    private bool isItemHeld = false;
    private Item heldItem;
    private int heldItemOriginalSortingOrder;
    private Vector3 currentMoveDirection = Vector3.zero;
    private float timeLastInteracted = 0;
    public bool IsItemHeld => isItemHeld;
    public Item HeldItem => heldItem;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
       if (PlayerInputHandler.Instance.Interact)
            Interact();  // If the button was released before it's considered a hold
    }

    private void FixedUpdate()
    {
        MovePlayer();
        UpdateSortingOrder(rend, transform);
        if (heldItem != null)
            UpdateSortingOrder(heldItem.Rend, heldItem.transform, 1);
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

    /// <summary>
    /// Ensures the bear is in the correct layer, to give the ilusion of the game being 3D
    /// </summary>
    private void UpdateSortingOrder(SpriteRenderer _rend, Transform _transform, int offset = 0)
    {
        _rend.sortingOrder = 100 - Mathf.RoundToInt(_transform.position.y) + offset;
    }
}