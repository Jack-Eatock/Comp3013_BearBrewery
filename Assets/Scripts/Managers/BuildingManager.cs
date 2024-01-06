using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DistilledGames
{
	public class BuildingManager : MonoBehaviour
	{
		public static BuildingManager Instance;
		public BuildingData SelectedBuilding;

		[SerializeField]
		private Tilemap tileMapFloor, tileMapGrid, tileMapWalls, tileMapPlacements;
		[SerializeField]
		private TileBase gridTile;
		private readonly Dictionary<Vector2Int, Building> placedObjects = new();

		private IEnumerator showingGrid;
		private readonly float gridTime = .3f;

		// Conveyer Belts
		private readonly Dictionary<Vector2Int, Conveyer> conveyers = new();
		private float timeOfLastTick = 0;

		public bool Running = true;

		#region Getters

		public Tilemap TileMapGrid => tileMapGrid;

		#endregion

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			GenerateGrid();
		}

		private void Update()
		{
			if (Running)
			{
				UpdatingConveyerBelts();
			}
		}

		/// <summary>
		/// Generate a grid by looking at where the floor tiles are placed.
		/// </summary>
		private void GenerateGrid()
		{
			for (int y = tileMapFloor.cellBounds.yMin; y < tileMapFloor.cellBounds.yMax; y++)
			{
				for (int x = tileMapFloor.cellBounds.xMin; x < tileMapFloor.cellBounds.xMax; x++)
				{
					//Debug.Log(tileMapFloor.GetTile(new Vector3Int(x, y, 0)) + " " + x + " " + y);

					TileBase tile = tileMapFloor.GetTile(new Vector3Int(x, y, 0));
					if (tile != null)
					{
						_ = new Vector3Int(x, y, 0);
						tileMapGrid.SetTile(new Vector3Int(x, y, 2), gridTile);
					}
				}
			}
		}

		/// <summary>
		/// Gets the closest grid Coord from the mouse.
		/// </summary>
		/// <returns></returns>
		public Vector3Int ClosestGridCoord()
		{
			Vector2 inputPos = PlayerInputHandler.Instance.PrimaryCursorPosition;
			Vector3 inputPosWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, 10));
			Vector3Int inputPosCelSpace = tileMapGrid.WorldToCell(inputPosWorldSpace);
			Vector3Int cell = new(inputPosCelSpace.x, inputPosCelSpace.y, 2);
			return cell;
		}

		public Vector3 GetWorldPosOfGridCoord(Vector3Int cell)
		{
			TileBase tile = tileMapGrid.GetTile(cell);
			if (tile != null)
			{
				return tileMapGrid.CellToWorld(cell);
			}

			Debug.LogWarning("Could not find tile!");
			return Vector3.zero;
		}

		/// <summary>
		/// Checks if a set off coords our within the bounds of the grid.
		/// </summary>
		/// <param name="coords"></param>
		/// <returns></returns>
		public bool OurCoordsWithinBounds(List<Vector3Int> coords)
		{
			for (int i = 0; i < coords.Count; i++)
			{
				if (!tileMapGrid.HasTile(coords[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Attempt to place an object at a specific coord. returns false if failed. true if successful.
		/// </summary>
		/// <param name="coords"></param>
		/// <param name="objToPlace"></param>
		/// <returns></returns>
		public bool PlaceObject(Vector2Int coords, Building objToPlace)
		{
			// Can they afford it?
			if (objToPlace.Data.Cost > GameManager.Instance.Cash)
			{
				return false;
			}

			if (!CanObjectBePlacedHere(coords, objToPlace))
			{
				return false;
			}

			Debug.Log("a  " + objToPlace + " " + objToPlace.Rend);
			Helper.UpdateSortingOrder(objToPlace.Rend, objToPlace.transform);
			placedObjects.Add(coords, objToPlace);

			if (objToPlace.TryGetComponent(out Conveyer conveyer))
			{
				conveyers.Add(coords, conveyer);
			}

			objToPlace.OnPlaced(coords);
			CalculateConveyerConnections();
			return true;
		}

		public void DeleteObject(Building objectToDelete)
		{
			Debug.Log("DELETE");
			foreach (KeyValuePair<Vector2Int, Building> keyPair in placedObjects)
			{
				if (keyPair.Value == objectToDelete)
				{
					if (objectToDelete.TryGetComponent(out Conveyer _))
					{
						_ = conveyers.Remove(keyPair.Key);
						CalculateConveyerConnections();
					}

					_ = placedObjects.Remove(keyPair.Key);
					objectToDelete.OnDeleted();
					GameObject.Destroy(objectToDelete.gameObject);
					return;
				}
			}
		}

		public bool GetObjectsCoords(Building building, out Vector2Int coords)
		{
			coords = Vector2Int.zero;
			foreach (KeyValuePair<Vector2Int, Building> keyValuePair in placedObjects)
			{
				if (keyValuePair.Value == building)
				{
					coords = keyValuePair.Key;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if a specific object can be placed at specific coords.
		/// </summary>
		/// <param name="coords"></param>
		/// <param name="objToPlace"></param>
		/// <returns></returns>
		private bool CanObjectBePlacedHere(Vector2Int coords, Building objToPlace)
		{
			// Check each coord it would cover.
			for (int y = 0; y < objToPlace.Data.Height; y++)
			{
				for (int x = 0; x < objToPlace.Data.Width; x++)
				{
					if (GetObjectAtCoords(coords + new Vector2Int(x, y), out _))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Gets the placed object at the coords or any objects that overlaps that coord.
		/// </summary>
		/// <param name="coords"></param>
		/// <param name="placement"></param>
		/// <returns></returns>
		public bool GetObjectAtCoords(Vector2Int coords, out Building placement)
		{
			// Object at exact coord
			if (placedObjects.TryGetValue(coords, out placement))
			{
				return true;
			}

			// Otherwise check all objects placed for the space they take up.
			foreach (KeyValuePair<Vector2Int, Building> placedObject in placedObjects)
			{
				// Check each cord the object takes up.
				for (int y = 0; y < placedObject.Value.Data.Height; y++)
				{
					for (int x = 0; x < placedObject.Value.Data.Width; x++)
					{
						if (coords == (placedObject.Key + new Vector2Int(x, y)))
						{
							placement = placedObject.Value;
							return true;
						}
					}
				}
			}
			return false;
		}

		public void ShowGrid(bool show)
		{
			if (showingGrid != null)
			{
				StopCoroutine(showingGrid);
			}

			showingGrid = ShowingGrid(show);
			_ = StartCoroutine(showingGrid);
		}

		public void ShowArrows(bool show)
		{
			foreach (KeyValuePair<Vector2Int, Building> obj in placedObjects)
			{
				obj.Value.ShowArrows(show);
			}
		}

		private IEnumerator ShowingGrid(bool show)
		{
			float timeStarted = Time.time;
			Color tmpColor = tileMapGrid.color;

			if (show)
			{
				if (tmpColor.a == 1)
				{
					yield break;
				}
			}
			else
			{
				if (tmpColor.a == 0)
				{
					yield break;
				}
			}

			while (Time.time - timeStarted <= gridTime)
			{
				float fractionComplete = (Time.time - timeStarted) / gridTime;

				tmpColor.a = show ? Mathf.Lerp(0, 1, fractionComplete) : Mathf.Lerp(1, 0, fractionComplete);

				tileMapGrid.color = tmpColor;

				yield return new WaitForEndOfFrame();
			}

			tmpColor.a = show ? 1 : 0;
			tileMapGrid.color = tmpColor;
		}

		private void CalculateConveyerConnections()
		{
			List<Vector2Int> coordsToCheck = new();
			List<IConveyerInteractable> outGoingConnections = new();
			foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
			{
				outGoingConnections.Clear();
				coordsToCheck.Clear();

				// Look at the conveyers coord.
				Vector2Int conveyerCord = keyValuePair.Key;
				Conveyer conveyer = keyValuePair.Value;

				// Is there a conveyer in the direction it is facing?
				Vector2Int coordInFront = conveyerCord + conveyer.CordsFromDirection(conveyer.GetDirection());
				if (conveyers.ContainsKey(coordInFront))
				{
					// there is a cord in front. Check that it is not facing towards us.
					Vector2Int theirFrontCord = coordInFront + conveyers[coordInFront].CordsFromDirection(conveyers[coordInFront].GetDirection());
					if (theirFrontCord != conveyer.GridCoords)
					{
						outGoingConnections.Add(conveyers[coordInFront]);
					}
				}
				else
				{
					// Check for a building.
					if (GetObjectAtCoords(coordInFront, out Building building))
					{
						if (building.TryGetComponent(out IConveyerInteractable conveyerInteractable))
						{
							if (conveyerInteractable.CanConnectIn(coordInFront))
							{
								outGoingConnections.Add(conveyerInteractable);
							}
						}
					}
				}

				// Conveyer to the left of facing
				Direction leftOfFacing = IterateDirection(conveyer.GetDirection(), -1);
				Vector2Int coordToLeft = conveyerCord + conveyer.CordsFromDirection(leftOfFacing);
				if (conveyers.ContainsKey(coordToLeft))
				{
					// We only care if it is also facing our version of "Left" (away from us)
					if (conveyers[coordToLeft].GetDirection() == leftOfFacing)
					{
						outGoingConnections.Add(conveyers[coordToLeft]);
					}
				}

				// Conveyer to the right of facing
				Direction rightOfFacing = IterateDirection(conveyer.GetDirection(), 1);
				Vector2Int coordToRight = conveyerCord + conveyer.CordsFromDirection(rightOfFacing);
				if (conveyers.ContainsKey(coordToRight))
				{
					// We only care if it is also facing our version of "Right" (away from us)
					if (conveyers[coordToRight].GetDirection() == rightOfFacing)
					{
						outGoingConnections.Add(conveyers[coordToRight]);
					}
				}

				conveyer.SetOutConnections(outGoingConnections);
			}
		}

		private Direction IterateDirection(Direction dir, int change)
		{
			return ((int)dir + change) > 3 ? 0 : ((int)dir + change) < 0 ? (Direction)3 : dir + change;
		}

		private void UpdatingConveyerBelts()
		{
			if (Time.time - timeOfLastTick < GameManager.Instance.ConveyerBeltsTimeToMove)
			{
				return;
			}

			// Each conveyer belt should prepare to send.
			foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
			{
				keyValuePair.Value.PrepareToSend();
			}

			// Each conveyer belt should send out anything they are holding onto.
			foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
			{
				keyValuePair.Value.Send();
			}

			// Each conveyer belt should pull out from a building if they can.
			foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
			{
				keyValuePair.Value.PullFromBuildings();
			}

			timeOfLastTick = Time.time;
		}
	}

	public interface IPlaceableObject
	{
		void OnPlaced(Vector2Int _gridCoords);
		void OnDeleted();
		bool Rotate(int dir);
		void ShowArrows(bool state);
	}
}
