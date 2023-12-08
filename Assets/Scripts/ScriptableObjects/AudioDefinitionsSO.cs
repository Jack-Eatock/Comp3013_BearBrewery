
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    [CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/AudioDefinitions", order = 1)]
    public class AudioDefinitionsSO : ScriptableObject
    {
        public List<AudioClipDefinition> definitions = new List<AudioClipDefinition>();
    }
}

