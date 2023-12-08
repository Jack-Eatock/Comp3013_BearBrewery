using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace DistilledGames
{
    public class DepositBox : Building, IInteractable,  IConveyerInteractable
    {
        [SerializeField]
        private List<Vector2Int> conveyerIn = new List<Vector2Int>();

        #region Player Interacting

        public bool TryToInsertItem(Item item, bool conveyer = false)
        {
            if (item != null)
            {
                int itemValue = item.SellValue;

                GameManager.Instance.EarnedCash(itemValue);
                Debug.Log("Item deposited with value: " + itemValue);
                Destroy(item.gameObject);

                return true;
            }
            return false;
        }


        public bool TryToRetreiveItem(out Item item)
        {
            item = null;
            return false;
        }

        #endregion

        public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
        {
            return true;
        }

        public bool CanConnectIn(Vector2Int coords)
        {
            Vector2Int gridCoordsAdjusted = Vector2Int.zero;
            for (int i = 0; i < conveyerIn.Count; i++)
            {
                gridCoordsAdjusted = GridCoords + conveyerIn[i];
                if (coords == gridCoordsAdjusted)
                    return true;
            }
            return false;
        }

        public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
        {
            if (item != null)
            {
                StartCoroutine(MoveItemFromConveyerIntoMachine(item, insertFromCoords));
                return true;
            }
            return false;
        }


        private IEnumerator MoveItemFromConveyerIntoMachine(Item item, Vector2Int inputCoords)
        {
            Vector3 startingPos = item.transform.position;
            float timeStarted = Time.time;
            float timeToMove = GameManager.Instance.ConveyerBeltsTimeToMove;
            Vector3 targetPos = BuildingManager.instance.GetWorldPosOfGridCoord(new Vector3Int(inputCoords.x, inputCoords.y, 2));
            targetPos.x += .5f;
            targetPos.y += .5f;

            while (Time.time - timeStarted < timeToMove)
            {
                float percentageComplete = (Time.time - timeStarted) / timeToMove;
                if (item != null)
                    item.transform.position = Vector3.Lerp(startingPos, targetPos, percentageComplete);
                else
                    yield break;
                yield return new WaitForEndOfFrame();
            }
            if (item != null)
                item.transform.position = targetPos;

            int itemValue = item.SellValue;
            GameManager.Instance.EarnedCash(itemValue);
            Debug.Log("Item deposited with value: " + itemValue);
            Destroy(item.gameObject);
        }


        public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
        {
            item = null;
            return false;
        }
    }
}

