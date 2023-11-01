using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositBox : MonoBehaviour
{
    public void DepositItem(Item item)
    {
        if (item != null)
        {
            int itemValue = item.Value;
            // Here, you can add logic to use the item value, e.g., adding to a player's score.

            Debug.Log("Item deposited with value: " + itemValue);

            // Delete the item
            Destroy(item.gameObject);
        }
    }
}