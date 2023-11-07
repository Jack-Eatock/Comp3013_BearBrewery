using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DistilledGames
{
    public class Combiner : Building, IInteractable, IConveyerInteractable
    {
        [SerializeField] private List<Recipe> recipes; // This will hold the references to the Recipe ScriptableObjects
        [SerializeField] private int maxCapacity;
        [SerializeField] private float processingTime;

        private Dictionary<int, List<Recipe>> recipeDictionary; // Maps an item ID to its recipes
        private Dictionary<int, int> inputCounts; // Tracks the counts of each input item ID
        private int outputItemCount = 0;
        private bool isProcessing = false;
        private List<Recipe.ItemCountPair> currentOutputItems;

        [SerializeField]
        private List<Vector2Int> conveyerIn, conveyerOut;

        private void Start()
        {
            recipeDictionary = new Dictionary<int, List<Recipe>>();
            inputCounts = new Dictionary<int, int>();

            // Iterate over each recipe to populate our dictionary
            foreach (Recipe recipe in recipes)
            {
                foreach (var inputItem in recipe.InputItems)
                {
                    if (!recipeDictionary.ContainsKey(inputItem.itemPrefab.ItemID))
                    {
                        recipeDictionary[inputItem.itemPrefab.ItemID] = new List<Recipe>();
                    }
                    recipeDictionary[inputItem.itemPrefab.ItemID].Add(recipe);

                    // Initialize inputCounts dictionary
                    inputCounts[inputItem.itemPrefab.ItemID] = 0;
                }
            }
        }

        void Update()
        {
            // Process items if not currently processing and there is a recipe that can be made
            if (!isProcessing && CanProcessAnyRecipe())
            {
                StartCoroutine(ProcessItems());
            }
        }

        private bool CanProcessAnyRecipe() // Is there a recipe that can be currently processed?
        {
            foreach (var recipe in recipes)
            {
                bool canMake = true;
                foreach (var inputItem in recipe.InputItems)
                {
                    if (inputCounts[inputItem.itemPrefab.ItemID] < inputItem.itemCount)
                    {
                        canMake = false;
                        break;
                    }
                }

                if (canMake)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator ProcessItems()
        {
            isProcessing = true;
            yield return new WaitForSeconds(processingTime);

            // Find and process a valid recipe
            foreach (var recipe in recipes)
            {
                bool canProcess = true;
                foreach (var input in recipe.InputItems)
                {
                    if (inputCounts[input.itemPrefab.ItemID] < input.itemCount)
                    {
                        canProcess = false;
                        break;
                    }
                }

                if (canProcess)
                {
                    // Consume the input items
                    foreach (var input in recipe.InputItems)
                    {
                        inputCounts[input.itemPrefab.ItemID] -= input.itemCount;
                    }

                    // Store outputs for retrieval
                    currentOutputItems = new List<Recipe.ItemCountPair>(recipe.OutputItems);
                    outputItemCount += recipe.OutputItems.Count;
                    break;
                }
            }

            isProcessing = false;
        }

        #region Player Interaction

        public bool TryToInsertItem(Item item)
        {
            if (recipeDictionary.TryGetValue(item.ItemID, out List<Recipe> possibleRecipes))
            {
                if (inputCounts[item.ItemID] < maxCapacity)
                {
                    inputCounts[item.ItemID]++;
                    Destroy(item.gameObject); // Consuming the item
                    Debug.Log("Item added to the combiner.");
                    return true;
                }
            }

            Debug.Log("Cannot add item to combiner. It may be full or not suitable for any recipe.");
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (currentOutputItems != null && currentOutputItems.Count > 0)
            {
                var outputPair = currentOutputItems[0];
                currentOutputItems.RemoveAt(0); // Remove the item from the list
                item = Instantiate(outputPair.itemPrefab); // Instantiate the output item
                Debug.Log("Item retrieved from combiner. Items left: " + outputItemCount);
                outputItemCount--;
                return true;
            }
            else
            {
                Debug.Log("No items to retrieve from combiner.");
                return false;
            }
        }
        #endregion

        #region Conveyers

        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            return TryToInsertItem(item);
        }

        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            item = null;
            Vector2Int gridCoordsAdjusted = Vector2Int.zero;
            for (int i = 0; i < conveyerOut.Count; i++)
            {
                gridCoordsAdjusted = GridCoords + conveyerOut[i];
                if (RetrieveFromCoords == gridCoordsAdjusted)
                    return TryToRetreiveItem(out item);
            }
            return false;
        }

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            if (recipeDictionary.TryGetValue(item.ItemID, out List<Recipe> possibleRecipes))
            {
                if (inputCounts[item.ItemID] < maxCapacity)
                    return true;
            }
            return false;
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            Vector2Int gridCoordsAdjusted = Vector2Int.zero;
            for (int i = 0; i < conveyerIn.Count; i++)
            {
                gridCoordsAdjusted = GridCoords + conveyerIn[i];
                if (coords == gridCoordsAdjusted)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
