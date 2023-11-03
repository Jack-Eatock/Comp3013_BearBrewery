using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [System.Serializable]
    public class BoilerRecipe
    {
        public Item inputItemPrefab;
        public Item outputItemPrefab;
    }

    public class Boiler : MonoBehaviour
    {
        [SerializeField] private List<BoilerRecipe> recipes; // List of input-output prefab pairs
        [SerializeField] private Collider2D inputCollider;
        [SerializeField] private Collider2D outputCollider;
        [SerializeField] private int maxCapacity;
        [SerializeField] private float processingTime; // in seconds

        private Dictionary<int, Item> recipeDictionary; // Dictionary to store input to output prefab mapping
        private int inputItemCount = 0;
        private int outputItemCount = 0;
        private bool isProcessing = false;
        private Item currentItem;
        private Item outputItemPrefab;

        private void Start()
        {
            recipeDictionary = new Dictionary<int, Item>();
            foreach (BoilerRecipe recipe in recipes)
            {
                if (!recipeDictionary.ContainsKey(recipe.inputItemPrefab.ItemID))
                {
                    recipeDictionary.Add(recipe.inputItemPrefab.ItemID, recipe.outputItemPrefab);
                }
                else
                {
                    Debug.LogWarning("Duplicate recipe detected for item ID: " + recipe.inputItemPrefab.ItemID);
                }
            }
        }

        void Update()
        {
            // If there are items and the boiler is not already processing, start the process
            if (inputItemCount > 0 && !isProcessing)
            {
                StartCoroutine(ProcessItem());
            }
        }

        public void AddItem(Item item)
        {
            // Check if the item can be processed and if there's space
            if (inputItemCount < maxCapacity && recipeDictionary.ContainsKey(item.ItemID) && (currentItem == null || item.ItemID == currentItem.ItemID))
            {
                inputItemCount++;
                currentItem = item;
                outputItemPrefab = recipeDictionary[item.ItemID]; // Use the ItemID to get the corresponding output prefab
                Destroy(item.gameObject); // Consuming the item
                Debug.Log("Item added to the boiler. Total items: " + inputItemCount);
            }
            else
            {
                Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
            }
        }

        public Item RemoveItem()
        {
            if (outputItemCount > 0)
            {
                outputItemCount--;
                Debug.Log("Item removed from boiler. Items left: " + outputItemCount);
                // Instantiate the output prefab using the reference stored during AddItem
                return Instantiate(outputItemPrefab, outputCollider.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("No items to remove from boiler.");
                return null;
            }
        }

        private IEnumerator ProcessItem()
        {
            isProcessing = true;
            yield return new WaitForSeconds(processingTime);

            // Convert input items to output items one by one
            inputItemCount--;
            outputItemCount++;
            Debug.Log("Processed one item. Items in boiler: " + inputItemCount + ". Items ready: " + outputItemCount);

            isProcessing = false;
        }
    }
}
