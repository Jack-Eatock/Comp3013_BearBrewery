using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [System.Serializable]
    public class BoilerRecipe
    {
        public Item inputItemPrefab;
        public int inputItemCount; // Amount of input items needed
        public Item outputItemPrefab;
        public int outputItemCount; // Amount of output items produced
    }

    public class Boiler : Building, IInteractable
    {
        [SerializeField] private List<BoilerRecipe> recipes; // List of input-output prefab pairs
        [SerializeField] private int maxCapacity;
        [SerializeField] private float processingTime; // in seconds

        private Dictionary<int, BoilerRecipe> recipeDictionary;
        private int inputItemCount = 0;
        private int outputItemCount = 0;
        private bool isProcessing = false;
        private Item currentItem;
        private BoilerRecipe currentRecipe;

        private void Start()
        {
            recipeDictionary = new Dictionary<int, BoilerRecipe>();
            foreach (BoilerRecipe recipe in recipes)
            {
                if (!recipeDictionary.ContainsKey(recipe.inputItemPrefab.ItemID))
                    recipeDictionary.Add(recipe.inputItemPrefab.ItemID, recipe);
                else
                    Debug.LogWarning("Duplicate recipe detected for item ID: " + recipe.inputItemPrefab.ItemID);
            }
        }

        void Update()
        {
            // If there are items and the boiler is not already processing, start the process
            if (inputItemCount >= (currentRecipe?.inputItemCount ?? 0) && !isProcessing)
            {
                StartCoroutine(ProcessItem());
            }
        }

        private IEnumerator ProcessItem()
        {
            isProcessing = true;
            yield return new WaitForSeconds(processingTime);

            if (currentRecipe != null)
            {
                inputItemCount -= currentRecipe.inputItemCount; // Consume input items
                outputItemCount += currentRecipe.outputItemCount; // Produce output items
                Debug.Log($"Processed items. Items in boiler: {inputItemCount}. Items ready: {outputItemCount}");
            }
            else
            {
                Debug.LogError("Current recipe is not set in the boiler.");
            }

            isProcessing = false;
        }

        public bool TryToInsertItem(Item item)
        {
            if (recipeDictionary.TryGetValue(item.ItemID, out BoilerRecipe recipe))
            {
                // Check for capacity and recipe requirement
                if (inputItemCount < maxCapacity && inputItemCount < recipe.inputItemCount)
                {
                    inputItemCount++;
                    currentItem = item;
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
                item = Instantiate(currentRecipe.outputItemPrefab); // Use the output prefab stored during item insertion
                Debug.Log("Item removed from boiler. Items left: " + outputItemCount);
                return true;
            }
            else
            {
                Debug.Log("No items to remove from boiler.");
                return false;
            }
        }
    }
}
