using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class Depot : Building, IInteractable
    {
        [SerializeField] private Item itemTypePrefab; // Prefab of the item type you want to generate
        [SerializeField] private float dispenseRate; // items per minute
        [SerializeField] private float itemsHeld;
        [SerializeField] private float maxHoldingAmount;

        private float timeSinceLastDispense = 0f;
        private float timeForNextDispense;

        void Start()
        {
            timeForNextDispense = 60f / dispenseRate; // converting the rate from per minute to per second
        }

        void Update()
        {
            if (itemsHeld < maxHoldingAmount)
            {
                timeSinceLastDispense += Time.deltaTime;
                if (timeSinceLastDispense >= timeForNextDispense)
                {
                    itemsHeld++;
                    timeSinceLastDispense = 0f;
                }
            }
        }

        public bool TryToInsertItem(Item item)
        {
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (itemsHeld > 0)
            {
                itemsHeld--;
                Debug.Log("Depot has: " + itemsHeld + " items remaining");
                item = Instantiate(itemTypePrefab, transform.position, Quaternion.identity); // creates the item at the Depot's position
                return true;
            }
            return false;
        }
    }
}
