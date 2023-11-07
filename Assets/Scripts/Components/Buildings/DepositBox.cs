using UnityEngine;

namespace DistilledGames
{
    public class DepositBox : Building, IInteractable
    {
        public bool TryToInsertItem(Item item)
        {
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
    }
}

