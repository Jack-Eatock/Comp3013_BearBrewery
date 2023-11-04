using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [System.Serializable]
    public class CombinerRecipe
    {
        public Item inputItemPrefab1;
        public Item inputItemPrefab2;
        public Item outputItemPrefab;
    }

    public class Combiner : Building, IInteractable
    {
        [SerializeField] private List<CombinerRecipe> recipes;
        [SerializeField] private int maxCapacity;
        [SerializeField] private float processingTime;

        private Dictionary<(int, int), Item> recipeDictionary;
        private int inputItem1Count = 0;
        private int inputItem2Count = 0;
        private int outputItemCount = 0;
        private bool isProcessing = false;
        private Item outputItemPrefab;

        private void Start()
        {
            recipeDictionary = new Dictionary<(int, int), Item>();
            foreach (CombinerRecipe recipe in recipes)
            {
                var key = (recipe.inputItemPrefab1.ItemID, recipe.inputItemPrefab2.ItemID);
                if (!recipeDictionary.ContainsKey(key))
                    recipeDictionary.Add(key, recipe.outputItemPrefab);
                else
                    Debug.LogWarning($"Duplicate recipe detected for item IDs: {recipe.inputItemPrefab1.ItemID} and {recipe.inputItemPrefab2.ItemID}");
            }
        }

        void Update()
        {
            if (inputItem1Count > 0 && inputItem2Count > 0 && !isProcessing)
                StartCoroutine(ProcessItems());
        }

        private IEnumerator ProcessItems()
        {
            isProcessing = true;
            yield return new WaitForSeconds(processingTime);

            inputItem1Count--;
            inputItem2Count--;
            outputItemCount++;
            Debug.Log($"Processed items. Items in combiner: {inputItem1Count} of type 1, {inputItem2Count} of type 2. Items ready: {outputItemCount}");

            isProcessing = false;
        }

        public bool TryToInsertItem(Item item)
        {
            bool itemAdded = false;

            foreach (var recipe in recipeDictionary)
            {
                if (item.ItemID == recipe.Key.Item1 || item.ItemID == recipe.Key.Item2)
                {
                    // Check if both inputs require the same item ID
                    if (recipe.Key.Item1 == recipe.Key.Item2)
                    {
                        // If so, distribute the items evenly across both inputs
                        if (inputItem1Count <= inputItem2Count && inputItem1Count < maxCapacity)
                        {
                            inputItem1Count++;
                            itemAdded = true;
                        }
                        else if (inputItem2Count < maxCapacity)
                        {
                            inputItem2Count++;
                            itemAdded = true;
                        }
                    }
                    else
                    {
                        // If the inputs require different items, just increment the correct one
                        if (item.ItemID == recipe.Key.Item1 && inputItem1Count < maxCapacity)
                        {
                            inputItem1Count++;
                            itemAdded = true;
                        }
                        else if (item.ItemID == recipe.Key.Item2 && inputItem2Count < maxCapacity)
                        {
                            inputItem2Count++;
                            itemAdded = true;
                        }
                    }

                    if (itemAdded)
                    {
                        outputItemPrefab = recipe.Value;
                        Destroy(item.gameObject); // Consuming the item
                        Debug.Log("Item added to the combiner.");
                        break; // Exit the loop if we've successfully added the item
                    }
                }
            }

            if (!itemAdded)
            {
                Debug.Log("Cannot add item to combiner. It may be full or not suitable for any recipe.");
            }

            return itemAdded;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (outputItemCount > 0)
            {
                outputItemCount--;
                Debug.Log("Item retrieved from combiner. Items left: " + outputItemCount);
                item = Instantiate(outputItemPrefab); // Use the output prefab stored during item insertion
                return true;
            }
            else
            {
                Debug.Log("No items to retrieve from combiner.");
                return false;
            }
        }
    }
}