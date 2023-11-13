using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class Depot : Building, IInteractable, IConveyerInteractable
    {
        [SerializeField] private Item itemTypePrefab; // Prefab of the item type you want to display
        [SerializeField] private Transform displayArea; // The transform that defines the display area
        [SerializeField] private BoxCollider2D displayAreaCollider; // Collider that defines the boundaries of the display area
        [SerializeField] private float dispenseRate; // items per minute
        [SerializeField] private float itemsHeld;
        [SerializeField] private float maxHoldingAmount;

        private List<Item> displayedItems = new List<Item>(); // Track displayed items
        private float timeSinceLastDispense = 0f;
        private float timeForNextDispense;

        void Start()
        {
            timeForNextDispense = 60f / dispenseRate; // converting the rate from per minute to per second
            UpdateDisplay(); // Initial update of the display
        }

        void Update()
        {
            if (itemsHeld < maxHoldingAmount)
            {
                timeSinceLastDispense += Time.deltaTime;
                if (timeSinceLastDispense >= timeForNextDispense)
                {
                    itemsHeld++;
                    timeSinceLastDispense = 0f;
                    UpdateDisplay();
                }
            }
        }

        private void UpdateDisplay()
        {
            // Get the sorting order of the depot's SpriteRenderer
            int depotSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;

            // Calculate the bounds of the display area
            Bounds displayBounds = displayAreaCollider.bounds;
            Vector2 spriteSize = itemTypePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;

            // Adjust the display bounds to account for the sprite size
            displayBounds.min += new Vector3(spriteSize.x * itemTypePrefab.transform.localScale.x / 2, spriteSize.y * itemTypePrefab.transform.localScale.y / 2, 0f);
            displayBounds.max -= new Vector3(spriteSize.x * itemTypePrefab.transform.localScale.x / 2, spriteSize.y * itemTypePrefab.transform.localScale.y / 2, 0f);

            // Add new items if necessary
            while (displayedItems.Count < Mathf.FloorToInt(itemsHeld))
            {
                // Generate a random position within the adjusted bounds
                float randomX = Random.Range(displayBounds.min.x, displayBounds.max.x);
                float randomY = Random.Range(displayBounds.min.y, displayBounds.max.y);
                Vector3 randomPosition = new Vector3(randomX, randomY, displayArea.position.z);

                // Instantiate the item at the random position within the bounds
                Item newItem = Instantiate(itemTypePrefab, randomPosition, Quaternion.identity, displayArea);
                newItem.SetInteractable(false);

                // Adjust the sorting order of the sprite renderer to be one higher than the depot's sorting order
                SpriteRenderer newItemRenderer = newItem.GetComponent<SpriteRenderer>();
                if (newItemRenderer != null)
                {
                    newItemRenderer.sortingOrder = depotSortingOrder + 1;
                }

                displayedItems.Add(newItem);
            }

            // Remove items if necessary
            while (displayedItems.Count > Mathf.FloorToInt(itemsHeld))
            {
                // Remove the last item in the list
                int lastIndex = displayedItems.Count - 1;
                Destroy(displayedItems[lastIndex].gameObject);
                displayedItems.RemoveAt(lastIndex);
            }
        }

        #region Player Interacting

        public bool TryToInsertItem(Item item)
        {
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (itemsHeld > 0)
            {
                itemsHeld--;
                Debug.Log("Depot has: " + itemsHeld + " items remaining");
                item = Instantiate(itemTypePrefab, transform.position, Quaternion.identity); // creates the item at the Depot's position
                item.SetInteractable(false);
                return true;
            }
            return false;
        }

        #endregion

        #region Conveyer Interactions

        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            return false;
        }

        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            return TryToRetreiveItem(out item);
        }

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            return false;
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            return false;
        }

        public override void OnDeleted()
        {

        }

        #endregion
    }
}
