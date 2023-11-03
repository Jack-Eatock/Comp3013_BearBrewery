using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DistilledGames
{
    [Serializable]
    public class Building : MonoBehaviour, IPlaceableObject
    {
        public BuildingData data;
        private SpriteRenderer renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void OnPlaced()
        {
            Debug.Log("Test");
        }

        public void UpdateSortingOrder()
        {
            renderer.sortingOrder = 100 - Mathf.RoundToInt(transform.position.y);
        }
    }
}
