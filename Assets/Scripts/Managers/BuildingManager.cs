using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DistilledGames
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager instance;
        public BuildingData selectedBuilding;

        [SerializeField]
        private Tilemap tileMapFloor, tileMapGrid, tileMapWalls, tileMapPlacements;
        [SerializeField]
        private TileBase gridTile;
        private Dictionary<Vector2Int, Building> placedObjects = new Dictionary<Vector2Int, Building>();

        private IEnumerator showingGrid;
        private float gridTime = .3f;

        // Conveyer Belts
        private Dictionary<Vector2Int, Conveyer> conveyers = new Dictionary<Vector2Int, Conveyer>();
        private float timeOfLastTick = 0;

        public bool Running = true;

        #region Getters

        public Tilemap TileMapGrid => tileMapGrid;

        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            GenerateGrid();
        }

        private void Update()
        {
        if (Running)
            UpdatingConveyerBelts();
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
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
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
            Vector3Int cell = new Vector3Int(inputPosCelSpace.x, inputPosCelSpace.y, 2);
            return cell;
        }

        public Vector3 GetWorldPosOfGridCoord(Vector3Int cell)
        {
            TileBase tile = tileMapGrid.GetTile(cell);
            if (tile != null)
                return tileMapGrid.CellToWorld(cell);
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
                    return false;
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
            if (objToPlace.data.Cost > GameManager.Instance.Cash)
                return false;

            if (!CanObjectBePlacedHere(coords, objToPlace))
                return false;

            Debug.Log("a  " + objToPlace + " " + objToPlace.Rend);
            Helper.UpdateSortingOrder(objToPlace.Rend, objToPlace.transform);
            placedObjects.Add(coords, objToPlace);

            Conveyer conveyer;
            if (objToPlace.TryGetComponent(out conveyer))
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
                    Conveyer conveyer;
                    if (objectToDelete.TryGetComponent(out conveyer))
                    {
                        conveyers.Remove(keyPair.Key);
                        CalculateConveyerConnections();
                    }
                     
                    placedObjects.Remove(keyPair.Key);
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
            Building obj;
            // Check each coord it would cover.
            for (int y = 0; y < objToPlace.data.Height; y++)
            {
                for (int x = 0; x < objToPlace.data.Width; x++)
                {
                    if (GetObjectAtCoords(coords + new Vector2Int(x, y), out obj))
                        return false;
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
                return true;

            // Otherwise check all objects placed for the space they take up.
            foreach (KeyValuePair<Vector2Int, Building> placedObject in placedObjects)
            {
                // Check each cord the object takes up.
                for (int y = 0; y < placedObject.Value.data.Height; y++)
                {
                    for (int x = 0; x < placedObject.Value.data.Width; x++)
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
                StopCoroutine(showingGrid);

            showingGrid = ShowingGrid(show);
            StartCoroutine(showingGrid);
        }

        public void ShowArrows(bool show)
        {
            foreach(KeyValuePair< Vector2Int, Building> obj in placedObjects)
                obj.Value.ShowArrows(show);
        }

        private IEnumerator ShowingGrid(bool show)
        {
            float timeStarted = Time.time;
            Color tmpColor = tileMapGrid.color;

            if (show)
            {
                if (tmpColor.a == 1)
                    yield break;
            }
            else
            {
                if (tmpColor.a == 0)
                    yield break;
            }

            while (Time.time - timeStarted <= gridTime)
            {
                float fractionComplete = (Time.time - timeStarted) / gridTime;

                if (show)
                    tmpColor.a = Mathf.Lerp(0, 1, fractionComplete);
                else
                    tmpColor.a = Mathf.Lerp(1, 0, fractionComplete);

                tileMapGrid.color = tmpColor;

                yield return new WaitForEndOfFrame();
            }

            if (show)
                tmpColor.a = 1;
            else
                tmpColor.a = 0;
            tileMapGrid.color = tmpColor;
        }

        private void CalculateConveyerConnections()
        {
            List<Vector2Int> coordsToCheck = new List<Vector2Int>();
            List<IConveyerInteractable> outGoingConnections = new List<IConveyerInteractable>();
            foreach(KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
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
                        outGoingConnections.Add(conveyers[coordInFront]);
                }
                else
                {
                    // Check for a building.
                    Building building = null;
                    if (GetObjectAtCoords(coordInFront, out building))
                    {
                        IConveyerInteractable conveyerInteractable = null;
                        if (building.TryGetComponent(out conveyerInteractable))
                        {
                            if (conveyerInteractable.CanConnectIn(coordInFront))
                                outGoingConnections.Add(conveyerInteractable);
                        }
                    }
                }

                // Conveyer to the left of facing
                Direction leftOfFacing = IterateDirection(conveyer.GetDirection(), - 1);
                Vector2Int coordToLeft = conveyerCord + conveyer.CordsFromDirection(leftOfFacing);
                if (conveyers.ContainsKey(coordToLeft))
                {
                    // We only care if it is also facing our version of "Left" (away from us)
                    if (conveyers[coordToLeft].GetDirection() == leftOfFacing)
                        outGoingConnections.Add(conveyers[coordToLeft]);
                }

                // Conveyer to the right of facing
                Direction rightOfFacing = IterateDirection(conveyer.GetDirection(), 1);
                Vector2Int coordToRight = conveyerCord + conveyer.CordsFromDirection(rightOfFacing);
                if (conveyers.ContainsKey(coordToRight))
                {
                    // We only care if it is also facing our version of "Right" (away from us)
                    if (conveyers[coordToRight].GetDirection() == rightOfFacing)
                        outGoingConnections.Add(conveyers[coordToRight]);
                }

                conveyer.SetOutConnections(outGoingConnections);
            }
        }

        private Direction IterateDirection(Direction dir, int change)
        {
            if (((int)dir + change) > 3)
                return (Direction)0;

            else if (((int) dir + change) < 0)
                return (Direction) 3;

            else
                return dir + change;
        }

        private void UpdatingConveyerBelts()
        {
            if (Time.time - timeOfLastTick < GameManager.Instance.ConveyerBeltsTimeToMove)
                return;

            // Each conveyer belt should prepare to send.
            foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
                keyValuePair.Value.PrepareToSend();

            // Each conveyer belt should send out anything they are holding onto.
            foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
                keyValuePair.Value.Send();

            // Each conveyer belt should pull out from a building if they can.
            foreach (KeyValuePair<Vector2Int, Conveyer> keyValuePair in conveyers)
                keyValuePair.Value.PullFromBuildings();

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
