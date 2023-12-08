using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public interface IInteractable
    {
        bool TryToInsertItem(Item item, bool conveyer = false);

        bool TryToRetreiveItem(out Item iten);
    }
}
