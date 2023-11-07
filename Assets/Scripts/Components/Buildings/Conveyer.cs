using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class Conveyer : Building, IInteractable, IConveyerInteractable
    {
        private Item itemOnBelt = null;
        private IConveyerInteractable sendingTo = null;

        [SerializeField]
        private List<IConveyerInteractable> outConnections = new List<IConveyerInteractable>();

        #region Conveyer Running Process

        public void SetOutConnections(List<IConveyerInteractable> outGoingConnections)
        {
            outConnections = new List<IConveyerInteractable>(outGoingConnections);
        }

        public void PrepareToSend()
        {
            if (itemOnBelt == null)
                return;

            IConveyerInteractable outGoing = null;

            // Randomise the outgoing connections
            List<IConveyerInteractable> tmpOutConnections = new List<IConveyerInteractable>(outConnections);
            IConveyerInteractable[] tmpOutGoing = new IConveyerInteractable[outConnections.Count];
            for (int i = 0; i < outConnections.Count; i++)
            {
                int randomIndex = Random.Range(0, tmpOutConnections.Count);
                tmpOutGoing[i] = tmpOutConnections[randomIndex];
                tmpOutConnections.RemoveAt(randomIndex);
            }

            // Pick an output.
            for (int i = 0; i < tmpOutGoing.Length; i++)
            {
                if (tmpOutGoing[i].CanAnItemBeInserted(itemOnBelt, gridCoords + CordsFromDirection(GetDirection())))
                {
                    outGoing = tmpOutGoing[i];
                    break;
                }
            }

            if (outGoing != null)
                sendingTo = outGoing;
        }

        public void Send()
        {
            if (sendingTo == null)
                return;

            if (sendingTo.ConveyerTryToInsertItem(itemOnBelt, gridCoords + CordsFromDirection(GetDirection())))
                itemOnBelt = null;

            sendingTo = null;
        }

        public void PullFromBuildings()
        {
            if (itemOnBelt != null)
                return;

            Vector2Int direction = CordsFromDirection(GetDirection());
            Building buildingBehind;
            if (BuildingManager.instance.GetObjectAtCoords((gridCoords - direction), out buildingBehind))
            {
                IConveyerInteractable conveyerInteractable = null;
                if (buildingBehind.TryGetComponent(out conveyerInteractable))
                {
                    Item item;
                    if (conveyerInteractable.ConveyerTryToRetrieveItem(gridCoords - direction, out item))
                        TryToInsertItem(item);
                }
            }
        }

        #endregion

        #region Interaction from Player

        public bool TryToInsertItem(Item item)
        {
            if (itemOnBelt == null)
            {
                itemOnBelt = item;
                itemOnBelt.transform.position = gameObject.transform.position;
                itemOnBelt.transform.parent = null;
                return true;
            }
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            if (itemOnBelt != null)
            {
                item = itemOnBelt;
                itemOnBelt = null;
                return true;
            }
            return false;
        }

        #endregion

        #region Interaction from other conveyer

        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            return TryToInsertItem(item);
        }

        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            // A conveyer should never have their item taken by another. They will handle moving themselves
            item = null;
            return false;
        }

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            return (itemOnBelt == null);
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            return false;
        }

        #endregion

        public override void OnDeleted()
        {
            Destroy(itemOnBelt.gameObject);
            itemOnBelt = null;
        }
    }
}
