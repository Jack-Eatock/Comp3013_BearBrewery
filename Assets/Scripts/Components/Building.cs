using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DistilledGames
{
    [Serializable]
    public class Building : MonoBehaviour, IPlaceableObject
    {
        [HideInInspector]
        public BuildingData data;
        private SpriteRenderer renderer;

        public SpriteRenderer Rend => renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void OnPlaced()
        {
            Debug.Log("Test");
        }
    }
}
