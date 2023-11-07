using DistilledGames;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Factory/Recipe")]
public class Recipe : ScriptableObject
{
    [System.Serializable]
    public struct ItemCountPair
    {
        public Item itemPrefab;
        public int itemCount;
    }

    [SerializeField] private List<ItemCountPair> inputItems;
    [SerializeField] private List<ItemCountPair> outputItems;

    public List<ItemCountPair> InputItems => inputItems;
    public List<ItemCountPair> OutputItems => outputItems;
}