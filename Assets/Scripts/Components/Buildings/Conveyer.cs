using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
	public class Conveyer : Building, IInteractable, IConveyerInteractable
	{
		public enum ConveyerType { Normal, Corner, Splitter, Merger }
		private ConveyerType type;

		public ConveyerType Type => type;

		private Item itemOnBelt = null;
		private IConveyerInteractable sendingTo = null;
		private IEnumerator movingItem = null;

		[SerializeField]
		private List<IConveyerInteractable> outConnections = new();

		[SerializeField]
		private Sprite curvedRotationRightUp, curvedRotationRightDown, curvedRotationLeftUp, curvedRotationLeftDown, curvedRotationUpRight, curvedRotationUpLeft, curvedRotationDownRight, curvedRotationDownLeft;

		private Direction directionIn;

		public void SetConveyerType(ConveyerType _type, Direction dirIn)
		{
			directionIn = dirIn;
			type = _type;
			UpdateSprite();
		}

		protected override Sprite GetRotationSprite(Direction dir)
		{
			switch (type)
			{
				case ConveyerType.Normal:
					return dir switch
					{
						Direction.Up => rotationUp,
						Direction.Right => rotationRight,
						Direction.Down => rotationDown,
						Direction.Left => rotationLeft,
						_ => null,
					};
			

				case ConveyerType.Corner:

					if (dir == Direction.Up)
					{
						if (directionIn == Direction.Right)
							return curvedRotationRightUp;
						else
							return curvedRotationLeftUp;
					}

					if (dir == Direction.Right)
					{
						if (directionIn == Direction.Up)
							return curvedRotationUpRight;
						else
							return curvedRotationDownRight;
					}

					if (dir == Direction.Down)
					{
						if (directionIn == Direction.Right)
							return curvedRotationRightDown;
						else
							return curvedRotationLeftDown;
					}

					if (dir == Direction.Left)
					{
						if (directionIn == Direction.Up)
							return curvedRotationUpLeft;
						else
							return curvedRotationDownLeft;
					}

					break;

			}
			return rotationUp;
		}

		#region Conveyer Running Process

		public void SetOutConnections(List<IConveyerInteractable> outGoingConnections)
		{
			outConnections = new List<IConveyerInteractable>(outGoingConnections);
		}

		public void PrepareToSend()
		{
			if (itemOnBelt == null)
			{
				return;
			}

			IConveyerInteractable outGoing = null;

			// Randomise the outgoing connections
			List<IConveyerInteractable> tmpOutConnections = new(outConnections);
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
			{
				sendingTo = outGoing;
			}
		}

		public void Send()
		{
			if (sendingTo == null || itemOnBelt == null)
			{
				return;
			}

			itemOnBelt.transform.position = gameObject.transform.position;
			if (sendingTo.ConveyerTryToInsertItem(itemOnBelt, gridCoords + CordsFromDirection(GetDirection())))
			{
				itemOnBelt = null;
			}

			sendingTo = null;
		}

		public void PullFromBuildings()
		{
			if (itemOnBelt != null)
			{
				return;
			}

			Vector2Int direction = CordsFromDirection(GetDirection());
			if (BuildingManager.Instance.GetObjectAtCoords(gridCoords - direction, out Building buildingBehind))
			{
				if (buildingBehind.TryGetComponent(out IConveyerInteractable conveyerInteractable))
				{
					if (conveyerInteractable.ConveyerTryToRetrieveItem(gridCoords - direction, out Item item))
					{
						if (itemOnBelt == null)
						{
							itemOnBelt = item;
							if (movingItem != null)
							{
								StopCoroutine(movingItem);
							}

							movingItem = MoveItemIntoConveyer(item);
							_ = StartCoroutine(movingItem);
						}
					}
				}
			}
		}

		#endregion

		#region Interaction from Player

		public bool TryToInsertItem(Item item, bool conveyer = false)
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
				if (movingItem != null)
				{
					StopCoroutine(movingItem);
				}

				item = itemOnBelt;
				itemOnBelt = null;
				return true;
			}
			return false;
		}

		#endregion

		private IEnumerator MoveItemIntoConveyer(Item item)
		{
			Vector3 startingPos = item.transform.position;
			float timeStarted = Time.time;
			float timeToMove = GameManager.Instance.ConveyerBeltsTimeToMove;
			while (Time.time - timeStarted < timeToMove)
			{
				float percentageComplete = (Time.time - timeStarted) / timeToMove;
				if (item != null)
				{
					item.transform.position = Vector3.Lerp(startingPos, gameObject.transform.position, percentageComplete);
				}
				else
				{
					yield break;
				}

				yield return new WaitForEndOfFrame();
			}
			if (item != null)
			{
				item.transform.position = gameObject.transform.position;
			}
		}

		#region Interaction from other conveyer

		public bool ConveyerTryToInsertItem(Item item, Vector2Int insertFromCoords)
		{
			if (itemOnBelt == null)
			{
				itemOnBelt = item;
				if (movingItem != null)
				{
					StopCoroutine(movingItem);
				}

				movingItem = MoveItemIntoConveyer(item);
				_ = StartCoroutine(movingItem);
				return true;
			}
			return false;
		}

		public bool ConveyerTryToRetrieveItem(Vector2Int RetrieveFromCoords, out Item item)
		{
			// A conveyer should never have their item taken by another. They will handle moving themselves
			item = null;
			return false;
		}

		public bool CanAnItemBeInserted(Item item, Vector2Int insertFromCoords)
		{
			return itemOnBelt == null;
		}

		public bool CanConnectIn(Vector2Int coords)
		{
			return false;
		}

		#endregion

		public override void OnDeleted()
		{
			if (itemOnBelt != null)
			{
				Destroy(itemOnBelt.gameObject);
			}

			itemOnBelt = null;
		}
	}
}
