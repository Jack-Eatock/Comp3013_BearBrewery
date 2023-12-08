using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Recipe;

namespace DistilledGames
{
    public class Boiler : Building, IInteractable, IConveyerInteractable
    {
        [SerializeField] private List<Recipe> recipes; // List of Scriptable Object recipes
        [SerializeField] private int maxRecipeItemsMulitplier; // Quanity of a recipee that can  fill at one time. A recipee that requires 1 of x and 2 of y with a maxCap as  2 would be allowed 2 and 4.
        [SerializeField] private float processingTime; // in seconds
        [SerializeField] private SpriteRenderer boilerSpriteRenderer; // Make sure to assign this in the Inspector.
        [SerializeField] private int maxOutputItems = 3;

        [SerializeField] private Sprite idleSprite; // Sprite when the boiler is not in use
        [SerializeField] private Sprite activeSprite; // Sprite when the boiler is in use

        private Dictionary<int, Recipe> recipeDictionary;
        private Dictionary<int, int> storedItems = new Dictionary<int, int>(); // Id , number of
        private Dictionary<int, int> outputItems = new Dictionary<int, int>(); 

        private bool isProcessing = false;
        private Recipe currentRecipe;

        [SerializeField]
        private Vector2Int conveyerIn, conveyerOut;

        [SerializeReference]
        private AudioClip runningSound, outputSound;
        private SFXInGame sfxController;


        protected override void Awake()
        {
            base.Awake();
            sfxController = GetComponent<SFXInGame>();
        }

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

            Debug.Log("Start: isProcessing = " + isProcessing);
        }

        void Update()
        {
            // If there are items and the boiler is not already processing, start the process
            if (CanWeProcess() && !isProcessing)
            {
                Debug.Log("Calling ProcessItem from Update.");
                StartCoroutine(ProcessItem());
            }
        }

        private void UpdateSprite()
        {
            // Check if there are items in the input or output 
            bool isActive = CanWeProcess();

            if (isActive)
                sfxController.LoopingClipPlay(runningSound, 1f);
            else
                sfxController.LoopingClipStop();

            // Update the sprite
            boilerSpriteRenderer.sprite = isActive ? activeSprite : idleSprite;
        }

        private bool CanWeProcess()
        {
            if (currentRecipe == null)
                return false;

            // Check the current recipe
            foreach (ItemCountPair pair in currentRecipe.InputItems)
            {
                // Do we have this item? 
                int numOfItem = NumOfItem(ref storedItems, pair.itemPrefab.ItemID);
                if (pair.itemCount > numOfItem)
                    return false;
            }

            // Make sure we are not full of output items#]
            if (NumOfItemsTotal(ref outputItems) + 1 > maxOutputItems)
                return false;

            return true;
        }

        private IEnumerator ProcessItem()
        {
            isProcessing = true;
            UpdateSprite();
            yield return new WaitForSeconds(processingTime);

            if (currentRecipe != null)
            {
                // Consume the correct number of input items
                foreach (ItemCountPair pair in currentRecipe.InputItems)
                {
                    for (int i = 0; i < pair.itemCount; i++)
                        RemoveItem(ref storedItems, pair.itemPrefab.ItemID);
                }

                // Add output items
                foreach (ItemCountPair pair in currentRecipe.OutputItems)
                {
                    for (int i = 0; i < pair.itemCount; i++)
                        AddItem(ref  outputItems, pair.itemPrefab.ItemID);
                }

                Debug.Log($"Processed items)");
            }

            isProcessing = false;
        }

        public bool TryToInsertItem(Item item)
        {
            // If the machine is empty. We set a new recipee.
            if (storedItems.Count == 0)
            {
                if (recipeDictionary.TryGetValue(item.ItemID, out Recipe recipe))
                    currentRecipe = recipe;
            }

            // We want to only take in items for this recipee.
            // Is this item in the recipee?
            if (IsItemInRecipee(item.ItemID))
            {
                // It is. But do we have enough space for it free?
                // Space per item is relevent to their ratio. 
                // How many of this item are in it already?
                int numOfItemInMachine = NumOfItem(ref storedItems, item.ItemID);
                int numOfItemsAllowed = MaxNumberOfItemAllowed(item.ItemID);

                Debug.Log("Num items" + numOfItemInMachine);
                Debug.Log(" Max items " + numOfItemsAllowed);
                // Is this number + 1 more than the needed amount for the recipee times the multiplier
                if (numOfItemInMachine + 1 <= numOfItemsAllowed)
                {
                    InsertItem(item);
                    return true;
                }
            }

            Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
            return false;
        }

        private void InsertItem(Item item)
        {
            AddItem(ref storedItems, item.ItemID);
            Destroy(item.gameObject);
            UpdateSprite(); // Update the sprite immediately after insertion
        }

        private int MaxNumberOfItemAllowed(int id)
        {
            // find its pair in the recipee
            for (int i = 0; i < currentRecipe.InputItems.Count; i++)
            {
                if (currentRecipe.InputItems[i].itemPrefab.ItemID == id)
                    return currentRecipe.InputItems[i].itemCount * maxRecipeItemsMulitplier;
            }
            return 0;
        }

        private int NumOfItem(ref Dictionary<int, int> _items, int id)
        {
            int num = 0;
            if (_items.TryGetValue(id, out num))
                return num;
            return num;
        }

        private int NumOfItemsTotal(ref Dictionary<int, int> _items)
        {
            int count = 0;
            foreach (KeyValuePair<int, int> pair in _items)
                count += pair.Value;
            return count;
        }

        private bool IsItemInRecipee(int id)
        {
            for (int i = 0; i < currentRecipe.InputItems.Count; i++)
            {
                if (currentRecipe.InputItems[i].itemPrefab.ItemID == id)
                    return true;
            }
            return false;
        }

        private void AddItem(ref Dictionary<int, int> _items, int id)
        {
            if (_items.ContainsKey(id))
                _items[id]++;
            else
                _items.Add(id, 1);
        }

        private void RemoveItem(ref Dictionary<int, int> _items, int id)
        {
            if (_items.ContainsKey(id))
            {
                if (_items[id] > 1)
                    _items[id]--;
                else
                    _items.Remove(id);
            }
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (outputItems.Count > 0)
            {
                KeyValuePair<int,int> itemPair = outputItems.First();
                ItemDefinition itemDefinition;
                if (GameManager.Instance.GameConfig.GetItemDefinitionById(itemPair.Key, out itemDefinition))
                {
                    // Remove item from output items
                    RemoveItem(ref outputItems, itemPair.Key);
                    item = Instantiate(itemDefinition.item);
                    item.SetInteractable(false);
                    return true;
                }
                else
                    Debug.LogError("Failed to get item definition from id!");
            }
            Debug.Log("No items to remove from boiler.");
            return false;
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

            if (TryToRetreiveItem(out item))
            {
                sfxController.PlayOneClip(outputSound, 1f);
                item.transform.position = gameObject.transform.position;
                return true;
            }
            return false;
        }

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            // If the machine is empty. We set a new recipee.
            if (storedItems.Count == 0)
                return true;

            if (IsItemInRecipee(item.ItemID))
            {
                int numOfItemInMachine = NumOfItem(ref  storedItems, item.ItemID);
                int numOfItemsAllowed = MaxNumberOfItemAllowed(item.ItemID);

                // Is this number + 1 more than the needed amount for the recipee times the multiplier
                if (numOfItemInMachine + 1 <= numOfItemsAllowed)
                    return true;
            }

            Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
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
