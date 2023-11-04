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

        private int currentRotation = 0;
        [SerializeField] private Sprite[] rotations;

        public SpriteRenderer Rend => renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void OnPlaced()
        {
            Debug.Log("Test");
        }

        public virtual bool Rotate()
        {
            if (rotations.Length > 0)
            {
                if (currentRotation + 1 >= rotations.Length)
                    currentRotation = 0;
                else
                    currentRotation++;
                renderer.sprite = rotations[currentRotation];
                return true;
            }
            return false;
        }
    }
}
