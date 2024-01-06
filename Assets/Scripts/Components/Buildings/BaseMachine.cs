using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Recipe;

namespace DistilledGames
{
	public class BaseMachine : Building, IInteractable, IConveyerInteractable
	{
		[SerializeField] private List<Recipe> recipes; // List of Scriptable Object recipes
		[SerializeField] private int maxRecipeItemsMulitplier; // Quanity of a recipee that can  fill at one time. A recipee that requires 1 of x and 2 of y with a maxCap as  2 would be allowed 2 and 4.
		[SerializeField] private float processingTime; // in seconds
		[SerializeField] private int maxOutputItems = 3;

		private readonly Dictionary<int, List<Recipe>> recipeDictionary = new(); // Input item. What recipes can it make? 
		private Dictionary<int, int> storedItems = new(); // Id , number of
		private Dictionary<int, int> outputItems = new();
		private bool isProcessing = false;
		private List<Recipe> currentRecipes = new();

		[SerializeField]
		private List<Vector2Int> conveyerIn, conveyerOut;

		protected virtual void Start()
		{
			foreach (Recipe recipeSO in recipes)
			{
				foreach (ItemCountPair inputItem in recipeSO.InputItems)
				{
					int inputID = inputItem.itemPrefab.ItemID;
					if (!recipeDictionary.ContainsKey(inputID))
					{
						recipeDictionary[inputID] = new List<Recipe>();
					}

					recipeDictionary[inputItem.itemPrefab.ItemID].Add(recipeSO);
				}
			}

			UpdateSprite(); // Set the initial sprite

			Debug.Log("Start: isProcessing = " + isProcessing);
		}

		protected virtual void Update()
		{
			// If there are items and the boiler is not already processing, start the process
			if (CanWeProcess() && !isProcessing)
			{
				Debug.Log("Calling ProcessItem from Update.");
				_ = StartCoroutine(ProcessItem());
			}
		}

		protected virtual void UpdateSprite()
		{

		}

		protected bool CanWeProcess()
		{
			if (!GetCurrentRecipe(out _))
			{
				return false;
			}

			// Make sure we are not full of output items#]
			return NumOfItemsTotal(ref outputItems) + 1 <= maxOutputItems;
		}

		protected virtual IEnumerator ProcessItem()
		{
			isProcessing = true;
			UpdateSprite();
			yield return new WaitForSeconds(processingTime);

			if (GetCurrentRecipe(out Recipe currentRecipe))
			{
				// Consume the correct number of input items
				foreach (ItemCountPair pair in currentRecipe.InputItems)
				{
					for (int i = 0; i < pair.itemCount; i++)
					{
						RemoveItem(ref storedItems, pair.itemPrefab.ItemID);
					}
				}

				// Add output items
				foreach (ItemCountPair pair in currentRecipe.OutputItems)
				{
					for (int i = 0; i < pair.itemCount; i++)
					{
						AddItem(ref outputItems, pair.itemPrefab.ItemID);
					}
				}

				Debug.Log($"Processed items)");
			}
			isProcessing = false;
		}

		private bool GetCurrentRecipe(out Recipe currentRecipe)
		{
			currentRecipe = null;
			bool canProcessRecipee = false;
			foreach (Recipe recipe in currentRecipes)
			{
				canProcessRecipee = true;
				// Check the current recipe
				foreach (ItemCountPair pair in recipe.InputItems)
				{
					// Do we have this item? 
					int numOfItem = NumOfItem(ref storedItems, pair.itemPrefab.ItemID);
					if (pair.itemCount > numOfItem)
					{
						canProcessRecipee = false;
						break;
					}
				}

				if (canProcessRecipee)
				{
					currentRecipe = recipe;
					break;
				}
			}

			return canProcessRecipee;
		}

		public virtual bool TryToInsertItem(Item item, bool conveyer = false)
		{
			// If the machine is empty. We set a new recipee.
			if (storedItems.Count == 0)
			{
				if (recipeDictionary.TryGetValue(item.ItemID, out List<Recipe> recipe))
				{
					currentRecipes = recipe;
				}
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

				//Debug.Log("Num items" + numOfItemInMachine);
				//Debug.Log(" Max items " + numOfItemsAllowed);
				// Is this number + 1 more than the needed amount for the recipee times the multiplier
				if (numOfItemInMachine + 1 <= numOfItemsAllowed)
				{
					InsertItem(item, conveyer);
					return true;
				}
			}

			Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
			return false;
		}

		protected virtual void InsertItem(Item item, bool dontDestroyItem = false)
		{
			AddItem(ref storedItems, item.ItemID);
			if (!dontDestroyItem)
			{
				Destroy(item.gameObject);
			}

			UpdateSprite(); // Update the sprite immediately after insertion
		}

		protected int MaxNumberOfItemAllowed(int id)
		{
			// Go through their available recipees and get the highest value for them.
			int highestNumber = 0;
			foreach (Recipe recipe in recipes)
			{
				// find its pair in the recipee
				for (int i = 0; i < recipe.InputItems.Count; i++)
				{
					if (recipe.InputItems[i].itemPrefab.ItemID == id)
					{
						if (recipe.InputItems[i].itemCount >= highestNumber)
						{
							highestNumber = recipe.InputItems[i].itemCount;
						}

						break;
					}
				}
			}

			return highestNumber * maxRecipeItemsMulitplier;
		}

		protected int NumOfItem(ref Dictionary<int, int> _items, int id)
		{
			return _items.TryGetValue(id, out int num) ? num : num;
		}

		protected int NumOfItemsTotal(ref Dictionary<int, int> _items)
		{
			int count = 0;
			foreach (KeyValuePair<int, int> pair in _items)
			{
				count += pair.Value;
			}

			return count;
		}

		protected bool IsItemInRecipee(int id)
		{
			foreach (Recipe recipe in recipes)
			{
				for (int i = 0; i < recipe.InputItems.Count; i++)
				{
					if (recipe.InputItems[i].itemPrefab.ItemID == id)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void AddItem(ref Dictionary<int, int> _items, int id)
		{
			if (_items.ContainsKey(id))
			{
				_items[id]++;
			}
			else
			{
				_items.Add(id, 1);
			}
		}

		protected void RemoveItem(ref Dictionary<int, int> _items, int id)
		{
			if (_items.ContainsKey(id))
			{
				if (_items[id] > 1)
				{
					_items[id]--;
				}
				else
				{
					_ = _items.Remove(id);
				}
			}
		}

		public virtual bool TryToRetreiveItem(out Item item)
		{
			item = null;
			if (outputItems.Count > 0)
			{
				KeyValuePair<int, int> itemPair = outputItems.First();
				if (GameManager.Instance.GameConfig.GetItemDefinitionById(itemPair.Key, out ItemDefinition itemDefinition))
				{
					// Remove item from output items
					RemoveItem(ref outputItems, itemPair.Key);
					item = Instantiate(itemDefinition.item);
					item.SetInteractable(false);
					return true;
				}
				else
				{
					Debug.LogError("Failed to get item definition from id!");
				}
			}
			Debug.Log("No items to remove from boiler.");
			return false;
		}

		#region Conveyer Belt

		protected virtual void OutputedToConveyer()
		{

		}

		protected virtual void InputtedFromConveyer()
		{

		}

		/// <summary>
		/// A conveyer belt is trying to input. Should it be able to??
		/// </summary>
		public virtual bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
		{
			// Try to take in the item
			if (TryToInsertItem(item, true))
			{
				_ = StartCoroutine(MoveItemFromConveyerIntoMachine(item, insertFromCoords));
				return true;
			}
			else
			{
				return false;
			}
		}

		private IEnumerator MoveItemFromConveyerIntoMachine(Item item, Vector2Int inputCoords)
		{
			Vector3 startingPos = item.transform.position;
			float timeStarted = Time.time;
			float timeToMove = GameManager.Instance.ConveyerBeltsTimeToMove;
			Vector3 targetPos = BuildingManager.Instance.GetWorldPosOfGridCoord(new Vector3Int(inputCoords.x, inputCoords.y, 2));
			targetPos.x += .5f;
			targetPos.y += .5f;

			while (Time.time - timeStarted < timeToMove)
			{
				float percentageComplete = (Time.time - timeStarted) / timeToMove;
				if (item != null)
				{
					item.transform.position = Vector3.Lerp(startingPos, targetPos, percentageComplete);
				}
				else
				{
					yield break;
				}

				yield return new WaitForEndOfFrame();
			}
			if (item != null)
			{
				item.transform.position = targetPos;
			}

			InputtedFromConveyer();
			Destroy(item.gameObject);
		}

		public virtual bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
		{
			item = null;

			// Are the requested coords lining up with the output coords.
			_ = Vector2Int.zero;
			for (int i = 0; i < conveyerOut.Count; i++)
			{
				Vector2Int gridCoordsAdjusted = GridCoords + conveyerOut[i];
				if (RetrieveFromCoords != gridCoordsAdjusted)
				{
					return false;
				}
			}

			if (TryToRetreiveItem(out item))
			{
				OutputedToConveyer();
				item.transform.position = gameObject.transform.position;
				return true;
			}
			return false;
		}

		public virtual bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
		{
			// If the machine is empty. We set a new recipee.
			if (storedItems.Count == 0)
			{
				return true;
			}

			if (IsItemInRecipee(item.ItemID))
			{
				int numOfItemInMachine = NumOfItem(ref storedItems, item.ItemID);
				int numOfItemsAllowed = MaxNumberOfItemAllowed(item.ItemID);

				// Is this number + 1 more than the needed amount for the recipee times the multiplier
				if (numOfItemInMachine + 1 <= numOfItemsAllowed)
				{
					return true;
				}
			}

			Debug.Log("Cannot add item to boiler. It may be full or not suitable for any recipe.");
			return false;
		}

		public virtual bool CanConnectIn(Vector2Int coords)
		{
			// Do the coords allign with the input coords.
			_ = Vector2Int.zero;
			for (int i = 0; i < conveyerIn.Count; i++)
			{
				Vector2Int gridCoordsAdjusted = GridCoords + conveyerIn[i];
				if (coords == gridCoordsAdjusted)
				{
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}

