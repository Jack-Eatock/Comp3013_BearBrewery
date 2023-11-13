using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class Depot : Building, IInteractable, IConveyerInteractable
    {
        [SerializeField] private Item itemTypePrefab; // Prefab of the item type you want to generate
        [SerializeField] private Transform displayRegion; // Center transform of the display region
        [SerializeField] private Vector2 displaySize; // Size of the display region
        [SerializeField] private float dispenseRate; // items per minute
        [SerializeField] private float itemsHeld;
        [SerializeField] private float maxHoldingAmount;

        private List<GameObject> displayedItems = new List<GameObject>(); // Track displayed items
        private float timeSinceLastDispense = 0f;
        private float timeForNextDispense;

        void Start()
        {
            timeForNextDispense = 60f / dispenseRate; // converting the rate from per minute to per second
            UpdateItemDisplay(); // Call this to update the display at start
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
                    UpdateItemDisplay();
                }
            }
        }

        private void UpdateItemDisplay()
        {
            // Destroy old representations
            foreach (var item in displayedItems)
            {
                Destroy(item);
            }
            displayedItems.Clear();

            // Instantiate new representations
            for (int i = 0; i < itemsHeld; i++)
            {
                Vector3 position = CalculateItemPosition(i);
                GameObject displayedItem = Instantiate(itemTypePrefab.gameObject, position, Quaternion.identity);
                displayedItem.SetInteractable(false); // Assuming SetInteractable is a method that disables interaction
                displayedItems.Add(displayedItem);
            }
        }

        private Vector3 CalculateItemPosition(int index)
        {
            // Logic to calculate where the item should be displayed within the region
            // This is a simple linear layout; you may want more complex positioning
            float xSpacing = displaySize.x / maxHoldingAmount; // Distance between each item
            return new Vector3(
                displayRegion.position.x + (index * xSpacing) - (displaySize.x * 0.5f) + (xSpacing * 0.5f),
                displayRegion.position.y,
                displayRegion.position.z
            );
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
