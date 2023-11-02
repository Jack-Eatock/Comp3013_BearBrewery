using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/Buildings", order = 1)]
    public class BuildingConfig : ScriptableObject
    {
        public BuildingData BuildingData;
    }
}


