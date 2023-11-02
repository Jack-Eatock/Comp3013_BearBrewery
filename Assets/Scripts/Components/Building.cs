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
        public void OnPlaced()
        {
            Debug.Log("Test");
        }
    }
}
