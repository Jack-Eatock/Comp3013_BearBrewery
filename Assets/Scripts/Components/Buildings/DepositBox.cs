using UnityEngine;
using System.Collections.Generic;

namespace DistilledGames
{
    public class DepositBox : Building, IInteractable,  IConveyerInteractable
    {
        [SerializeField]
        private List<Vector2Int> conveyerIn = new List<Vector2Int>();

        #region Player Interacting

        public bool TryToInsertItem(Item item)
        {
            if (item != null)
            {
                int itemValue = item.SellValue;

                GameManager.Instance.EarnedCash(itemValue);
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
            Vector2Int gridCoordsAdjusted = Vector2Int.zero;
            for (int i = 0; i < conveyerIn.Count; i++)
            {
                gridCoordsAdjusted = GridCoords + conveyerIn[i];
                if (coords == gridCoordsAdjusted)
                    return true;
            }
            return false;
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

