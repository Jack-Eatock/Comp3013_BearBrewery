using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public interface IConveyerInteractable
    {
        bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords);
        bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item);
        bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords);
        bool CanConnectIn(Vector2Int coords);
    }
}
