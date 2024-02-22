using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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

		public void PlacingBuildingUpdatePos(ref Building buildingPlacing, Vector3Int closestCoord)
		{
			Vector3 offset = new(BuildingManager.Instance.TileMapGrid.cellSize.x / 2 * buildingPlacing.Data.Width, BuildingManager.Instance.TileMapGrid.cellSize.y / 2 * buildingPlacing.Data.Height, 0);
			buildingPlacing.transform.position = BuildingManager.Instance.GetWorldPosOfGridCoord(closestCoord) + offset;

			if (buildingPlacing.TryGetComponent(out Conveyer conveyer))
			{
				AddConveyerConnection((Vector2Int)closestCoord, conveyer, true);
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
				AddConveyerConnection(coords, conveyer);
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

		/// <summary>
		/// Checks if there is a placed object at the coords or any objects that overlaps that coord.
		/// </summary>
		/// <param name="coords"></param>
		/// <param name="placement"></param>
		/// <returns></returns>
		public bool CheckIfObjectAtCoords(Vector2Int coords)
		{
			// Object at exact coord
			if (placedObjects.TryGetValue(coords, out Building placement))
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
							return true;
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

		private void AddConveyerConnection(Vector2Int conveyerCord, Conveyer conveyer, bool dontSave = false)
		{
			List<IConveyerInteractable> outGoingConnections = new();
			Conveyer tmpConveyer = conveyer;
			Vector2Int tmpVector;

			Vector2Int coordFront = conveyerCord + conveyer.CordsFromDirection(conveyer.GetDirection());
			Vector2Int coordRight = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 1));
			Vector2Int coordBack  = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 2));
			Vector2Int coordLeft  = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 3));

			Direction tmpDirection;
			if (IsSplitter(ref tmpConveyer, out tmpDirection))
			{
				conveyer.SetConveyerType(Conveyer.ConveyerType.Splitter, tmpDirection);
			}

			// Curved - No input from the back, 1 input from the side.
			else if (IsCurved(ref tmpConveyer))
			{
				conveyer.SetConveyerType(Conveyer.ConveyerType.Corner, tmpConveyer.GetDirection());
			}
			else
			{
				conveyer.SetConveyerType(Conveyer.ConveyerType.Normal, tmpConveyer.GetDirection());
			}

			if (!dontSave)
				conveyers.Add(conveyerCord,conveyer);

			bool IsCurved(ref Conveyer conveyerIn)
			{
			
				// No input from back
				if (conveyers.ContainsKey(coordBack) && conveyers.TryGetValue(coordBack, out tmpConveyer))
				{
					if (tmpConveyer.GetDirection() == conveyer.GetDirection()) // The conveyer behind this one is point same direction. It is not a curve
						return false;
				}

				bool inputFromLeft = false, inputFromRight = false;

				// Check left - See if the left conveyer is facing towards our conveyer.
				if (conveyers.ContainsKey(coordLeft) && conveyers.TryGetValue(coordLeft, out tmpConveyer))
				{
					tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
						inputFromLeft = true;
				}

				// Check right - See if the right conveyer is facing towards our conveyer.
				if (conveyers.ContainsKey(coordRight) && conveyers.TryGetValue(coordRight, out tmpConveyer))
				{
					tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
						inputFromRight = true;
				}

				if (inputFromLeft && !inputFromRight)
				{
					conveyerIn = conveyers[coordLeft];
					return true;
				}

				if (inputFromRight && !inputFromLeft)
				{
					conveyerIn = conveyers[coordRight];
					return true;
				}

				return false;
			}

			bool IsSplitter(ref Conveyer conveyerIn, out Direction targetDirection)
			{
				// Two nodes out
				// Figure out how many nodes outwards
				targetDirection = Direction.Up;
				List<Conveyer> outputs = new List<Conveyer>();
				List<Conveyer> inputs = new List<Conveyer>();
				Conveyer tmpConveyer2 = tmpConveyer;
				if (conveyers.ContainsKey(coordFront) && conveyers.TryGetValue(coordFront, out tmpConveyer))
				{
					// See if it is pointing away from us
					tmpVector = tmpConveyer.GridCoords - tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
					{
						outputs.Add(tmpConveyer);
					}
					else
					{
						tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
						if (tmpVector == conveyerCord)
						{
							tmpConveyer2 = tmpConveyer;
							inputs.Add(tmpConveyer);
						}
							
					}
				}

				if (conveyers.ContainsKey(coordRight) && conveyers.TryGetValue(coordRight, out tmpConveyer))
				{
					// See if it is pointing away from us
					tmpVector = tmpConveyer.GridCoords - tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
					{
						outputs.Add(tmpConveyer);
					}
					else
					{
						tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
						if (tmpVector == conveyerCord)
						{
							inputs.Add(tmpConveyer);
							tmpConveyer2 = tmpConveyer;
						}
					}
				}

				if (conveyers.ContainsKey(coordBack) && conveyers.TryGetValue(coordBack, out tmpConveyer))
				{
					// See if it is pointing away from us
					tmpVector = tmpConveyer.GridCoords - tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
					{
						outputs.Add(tmpConveyer);
					}
					else
					{
						tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
						if (tmpVector == conveyerCord) 
						{
							tmpConveyer2 = tmpConveyer;
							inputs.Add(tmpConveyer);
						}
					}
				}

				if (conveyers.ContainsKey(coordLeft) && conveyers.TryGetValue(coordLeft, out tmpConveyer))
				{
					// See if it is pointing away from us
					tmpVector = tmpConveyer.GridCoords - tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
					if (tmpVector == conveyerCord)
					{
						outputs.Add(tmpConveyer);
					}
					else
					{
						tmpVector = tmpConveyer.GridCoords + tmpConveyer.CordsFromDirection(tmpConveyer.GetDirection());
						if (tmpVector == conveyerCord)
						{
							inputs.Add(tmpConveyer);
							tmpConveyer2 = tmpConveyer;
						}
					}
				}

				if (inputs.Count == 1 && outputs.Count == 2)
				{
					bool sameDir = false;
					int whichWasNotSame = 0;

					for (int i = 0; i < outputs.Count; i++)
					{
						if (outputs[i].GetDirection() == inputs[0].GetDirection())
							sameDir = true;
						else
							whichWasNotSame = i;
					}

					if (sameDir)
					{
						targetDirection = IterateDirection(outputs[whichWasNotSame].GetDirection(), 2);
					}
					else
					{
						targetDirection = tmpConveyer2.GetDirection();
					}

					conveyerIn = tmpConveyer2;
					return true;
				}

				return false;
			}
		}

		private void CalculateConveyerConnections()
		{
			List<IConveyerInteractable> outGoingConnections = new();
			foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
			{
				outGoingConnections.Clear();

				// Look at the conveyers coord.
				Vector2Int conveyerCord = keyValuePair.Key;
				Conveyer conveyer = keyValuePair.Value;
				Building tmpBuilding;

				Vector2Int coordFront  = conveyerCord + conveyer.CordsFromDirection(conveyer.GetDirection());
				Vector2Int coordRight  = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 1));
				Vector2Int coordBack   = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 2));
				Vector2Int coordLeft   = conveyerCord + conveyer.CordsFromDirection(IterateDirection(conveyer.GetDirection(), 3));

				bool isFront, isRight, isBack, isLeft;
				isFront = GetObjectAtCoords(coordFront, out tmpBuilding);
				isRight = GetObjectAtCoords(coordRight, out tmpBuilding);
				isBack  = GetObjectAtCoords(coordBack,  out tmpBuilding);
				isLeft  = GetObjectAtCoords(coordLeft,  out tmpBuilding);

				if (conveyer.Type == Conveyer.ConveyerType.Normal)
				{
					// Is there a conveyer in the direction it is facing?
					if (conveyers.ContainsKey(coordFront))
					{
						// there is a cord in front. Check that it is not facing towards us.
						Vector2Int theirFrontCord = coordFront + conveyers[coordFront].CordsFromDirection(conveyers[coordFront].GetDirection());
						if (theirFrontCord != conveyer.GridCoords)
							outGoingConnections.Add(conveyers[coordFront]);
					}
					else
					{
						// Check for a building.
						if (GetObjectAtCoords(coordFront, out tmpBuilding))
						{
							if (tmpBuilding.TryGetComponent(out IConveyerInteractable conveyerInteractable))
							{
								if (conveyerInteractable.CanConnectIn(coordFront))
									outGoingConnections.Add(conveyerInteractable);
							}
						}
					}

					// Is this one a corner. Is there not a normal into this one
					// Connection behind
					
				}
				conveyer.SetOutConnections(outGoingConnections);
			}
		}

		private Direction IterateDirection(Direction dir, int change)
		{
			int result = (int)dir + change;
			if (result > 3)
				result = result % 4;

			else if (result < 0)
				result = 4 - Math.Abs(result) % 4;


			Direction dirResult = (Direction)result;

			return dirResult;
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
