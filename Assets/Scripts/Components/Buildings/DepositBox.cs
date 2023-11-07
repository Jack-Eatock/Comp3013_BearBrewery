using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DistilledGames
{
    public class DepositBox : Building, IInteractable,  IConveyerInteractable
    {
        #region Player Interacting

        public bool TryToInsertItem(Item item)
        {
            Debug.Log("aaa");
            if (item != null)
            {
                int itemValue = item.Value;
                // Here, you can add logic to use the item value, e.g., adding to a player's score.

                Debug.Log("Item deposited with value: " + itemValue);

                // Delete the item
                Destroy(item.gameObject);
                return true;
            }
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            return false;
        }

        #endregion

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            return true;
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            Debug.Log(coords == GridCoords);
            return coords == GridCoords;
        }

        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            return TryToInsertItem(item);
        }

        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            item = null;
            return false;
        }
    }
}

