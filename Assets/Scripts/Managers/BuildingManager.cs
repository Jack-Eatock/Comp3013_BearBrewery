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
            if (!CanObjectBePlacedHere(coords, objToPlace))
                return false;

            Helper.UpdateSortingOrder(objToPlace.Rend, objToPlace.transform);
            placedObjects.Add(coords, objToPlace);
            return true;
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
        private bool GetObjectAtCoords(Vector2Int coords, out Building placement)
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
    }

    public interface IPlaceableObject
    {
        void OnPlaced();

        bool Rotate();
    }
}
