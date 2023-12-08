using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DistilledGames.States
{
    public class BuildingModePlacing : BaseState
    {
        private Building buildingPlacing;
        private List<Vector3Int> requiredCoords = new List<Vector3Int>();
        private Vector3Int currentSelectedCoords;
        private float timeEntered;
        private bool rotated = false;
        private Direction direction = Direction.Up;

        public override void StateEnter()
        {
            base.StateEnter();
            timeEntered = Time.time;
            SpawnBuilding();

            if (gameManager.PrevState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.PrevState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
                return;
            BuildingManager.instance.Running = false;
            // Camera.main.fieldOfView
            GameManager.Instance.SwitchToCamController(true);
            BuildingManager.instance.ShowGrid(true);
            BuildingManager.instance.ShowArrows(true);
            gameManager.SetBearActive(false);
            gameManager.SetItemsActive(false);
            MenuManager.Instance.ShowMenu(MenuManager.Menus.BuildingMenu);
        }

        public override void StateExit()
        {
            GameObject.Destroy(buildingPlacing.gameObject);
            base.StateExit();

            if (gameManager.NextState == StateDefinitions.GameStates.BuildingMode.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModePlacing.ToString() || gameManager.NextState == StateDefinitions.GameStates.BuildingModeDeleting.ToString())
                return;

            BuildingManager.instance.Running = true;
            GameManager.Instance.SwitchToCamController(false);
            BuildingManager.instance.ShowGrid(false);
            BuildingManager.instance.ShowArrows(false);
            gameManager.SetBearActive(true);
            gameManager.SetItemsActive(true);
            MenuManager.Instance.HideMenu(MenuManager.Menus.BuildingMenu);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (buildingPlacing == null)
                return;

            Vector3Int closestCoord = BuildingManager.instance.ClosestGridCoord();

            Helper.UpdateSortingOrder(buildingPlacing.Rend, buildingPlacing.transform);

            // What coords would be required?
            requiredCoords.Clear();
            for (int width = 0; width < buildingPlacing.data.Width; width++)
                for (int height = 0; height < buildingPlacing.data.Height; height++)
                    requiredCoords.Add(closestCoord + new Vector3Int(width, height, 0));

            // Are all these coords within bounds?
            if (BuildingManager.instance.OurCoordsWithinBounds(requiredCoords))
            {
                currentSelectedCoords = closestCoord;
                Vector3 offset = new Vector3((BuildingManager.instance.TileMapGrid.cellSize.x / 2) * buildingPlacing.data.Width, (BuildingManager.instance.TileMapGrid.cellSize.y / 2) * buildingPlacing.data.Height, 0);
                buildingPlacing.transform.position = BuildingManager.instance.GetWorldPosOfGridCoord(closestCoord) + offset; 
            }
        }

        public override StateDefinitions.ChangeInState PrimaryInteractionPressed()
        {
            if (buildingPlacing == null)
                return StateDefinitions.ChangeInState.NoChange;

            Debug.Log("Try to place object");

            if (!EventSystem.current.IsPointerOverGameObject() && BuildingManager.instance.PlaceObject(new Vector2Int(currentSelectedCoords.x, currentSelectedCoords.y), buildingPlacing))
            {
                GameManager.Instance.SpentCash(buildingPlacing.data.Cost);
                BuildingMenu.instance.RefreshCosts();
                Debug.Log("placed");
                AudioManager.instance.SFX_PlayClip("Placed", 1f);
                // Placing multiple?
                SpawnBuilding();
                return StateDefinitions.ChangeInState.NoChange;
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Cant place here");
                buildingPlacing.FlashColour(Color.red, .2f);
                AudioManager.instance.SFX_PlayClip("CantPlace", 1f);
            }
          

            return StateDefinitions.ChangeInState.NoChange;
        }

        private void SpawnBuilding()
        {
            buildingPlacing = GameObject.Instantiate(BuildingManager.instance.selectedBuilding.BuidlingPrefab);
            buildingPlacing.data = BuildingManager.instance.selectedBuilding;
            buildingPlacing.ShowArrows(true);

            if (rotated)
                buildingPlacing.SetRotation(direction);
        }

        public override StateDefinitions.ChangeInState MovementInput(Vector2 input)
        {
            GameManager.Instance.CamController.OnMove(input);
            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState EnterBuildMode()
        {
            if (Time.time - timeEntered <= .5f)
                return StateDefinitions.ChangeInState.NoChange;
            GameObject.Destroy(buildingPlacing.gameObject);

            gameManager.NextState = StateDefinitions.GameStates.Normal.ToString();
            return StateDefinitions.ChangeInState.NextState;
        }

        public override StateDefinitions.ChangeInState RotateInput(int dir)
        {
            if (buildingPlacing == null)
                return StateDefinitions.ChangeInState.NoChange;

            // Try to rotate building
            if (buildingPlacing.Rotate(dir))
            {
                rotated = true;
                direction = buildingPlacing.GetDirection();
                Debug.Log("Rotated");
            }
            else
                Debug.Log("Cant rotate");
            return StateDefinitions.ChangeInState.NoChange;
        }
    }
}

