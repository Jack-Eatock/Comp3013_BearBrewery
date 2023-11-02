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

    [CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Buildings", order = 1)]
    public class BuildingData : ScriptableObject
    {
        public string Name = "Building";
        public bool Rotatable = false;
        public float Cost = 0;
        public int Width, Height;
        public Sprite DisplayImage;
        public Building BuidlingPrefab;
    }
}
