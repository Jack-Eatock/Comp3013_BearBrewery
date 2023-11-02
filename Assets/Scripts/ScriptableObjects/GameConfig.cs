using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public List<BuildingData> BuildingData;
    }
}


