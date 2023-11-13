using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class Boiler : Building, IInteractable, IConveyerInteractable
    {
        [SerializeField] private List<Recipe> recipes; // List of Scriptable Object recipes
        [SerializeField] private int maxCapacity;
        [SerializeField] private float processingTime; // in seconds
        [SerializeField] private SpriteRenderer boilerSpriteRenderer; // Make sure to assign this in the Inspector.

        [SerializeField] private Sprite idleSprite; // Sprite when the boiler is not in use
        [SerializeField] private Sprite activeSprite; // Sprite when the boiler is in use

        private Dictionary<int, Recipe> recipeDictionary;
        private int inputItemCount = 0; // You may want to handle this differently depending on your item stack logic
        private int outputItemCount = 0; // This also might need changing according to stack logic
        private bool isProcessing = false;
        private Recipe currentRecipe;
        private Renderer boilerRenderer; // Renderer to change the texture

        [SerializeField]
        private Vector2Int conveyerIn, conveyerOut;

        private void Start()
        {
            recipeDictionary = new Dictionary<int, Recipe>();
            foreach (Recipe recipeSO in recipes)
            {
                int inputID = recipeSO.InputItems[0].itemPrefab.ItemID; // Assumes the boiler only uses the first input item for processing
                if (!recipeDictionary.ContainsKey(inputID))
                    recipeDictionary.Add(inputID, recipeSO);
                else
                    Debug.LogWarning("Duplicate recipe detected for item ID: " + inputID);
            }

            if (boilerSpriteRenderer == null)
                boilerSpriteRenderer = GetComponent<SpriteRenderer>();

            UpdateSprite(); // Set the initial sprite
        }

        void Update()
        {
            // If there are items and the boiler is not already processing, start the process
            if (inputItemCount >= (currentRecipe?.InputItems[0].itemCount ?? 0) && !isProcessing)
            {
                StartCoroutine(ProcessItem());
            }

            UpdateSprite(); // Update the texture based on the boiler's state
        }

        private void UpdateSprite()
        {
            // Check if there are items in the input or output 
            bool isActive = inputItemCount > 0 || outputItemCount > 0;

            // Update the sprite
            boilerSpriteRenderer.sprite = isActive ? activeSprite : idleSprite;
        }

        private IEnumerator ProcessItem()
        {
            isProcessing = true;
            yield return new WaitForSeconds(processingTime);

            if (currentRecipe != null)
            {
                inputItemCount -= currentRecipe.InputItems[0].itemCount; // Consume input items
                outputItemCount += currentRecipe.OutputItems[0].itemCount; // Produce output items (assuming one-to-one processing for simplicity)
                Debug.Log($"Processed items. Items in boiler: {inputItemCount}. Items ready: {outputItemCount}");
            }
            else
            {
                // It is okay if the recipe is not set yet.
                //Debug.LogError("Current recipe is not set in the boiler.");
            }

            isProcessing = false;
        }

        public bool TryToInsertItem(Item item)
        {
            if (recipeDictionary.TryGetValue(item.ItemID, out Recipe recipe))
            {
                // Check for capacity and recipe requirement
                if (inputItemCount < maxCapacity && inputItemCount < recipe.InputItems[0].itemCount)
                {
                    inputItemCount++;
                    currentRecipe = recipe; // Set the current recipe
                    Destroy(item.gameObject); // Consuming the item
                    Debug.Log("Item added to the boiler. Total items: " + inputItemCount);
                    return true;
                }
            }

            Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (outputItemCount > 0)
            {
                outputItemCount--;
                item = Instantiate(currentRecipe.OutputItems[0].itemPrefab); // Use the output prefab stored during item insertion
                item.SetInteractable(false);
                Debug.Log("Item removed from boiler. Items left: " + outputItemCount);
                return true;
            }
            else
            {
                Debug.Log("No items to remove from boiler.");
                return false;
            }
        }

        #region Conveyer Belt

        /// <summary>
        /// A conveyer belt is trying to input. Should it be able to??
        /// </summary>
        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            // Try to take in the item
            return TryToInsertItem(item);
        }

        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            item = null;

            // Are the requested coords lining up with the output coords.
            Vector2Int outputCoords = gridCoords + conveyerOut;
            if (RetrieveFromCoords != outputCoords)
                return false;

            // Do we have items to output??
            if (outputItemCount <= 0)
                return false;

            item = Instantiate(currentRecipe.OutputItems[0].itemPrefab); // Use the output prefab stored during item insertion
            item.SetInteractable(false);
            item.transform.position = gameObject.transform.position;
            outputItemCount--;
            return true;
        }

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            if (recipeDictionary.TryGetValue(item.ItemID, out Recipe recipe))
            {
                // Check for capacity and recipe requirement
                if (inputItemCount < maxCapacity && inputItemCount < recipe.InputItems[0].itemCount)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            // Do the coords allign with the input coords.
            Vector2Int inputCoords = gridCoords + conveyerIn;
            if (inputCoords != coords)
                return false;
            return true;
        }

        #endregion
    }
}
