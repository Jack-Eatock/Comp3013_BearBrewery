using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public List<BuildingData> BuildingData;
        public List<ItemDefinition> ItemDefinitions;

        public bool GetItemDefinitionById(int id, out ItemDefinition outDef)
        {
            outDef = new ItemDefinition();
            foreach (ItemDefinition def in ItemDefinitions)
            {
                if (def.id == id)
                {
                    outDef = def;
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public struct ItemDefinition
    {
        public Item item;
        public int id;
    }
}


