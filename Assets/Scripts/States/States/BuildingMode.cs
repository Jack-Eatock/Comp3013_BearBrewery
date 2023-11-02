using UnityEngine;
using System.Collections.Generic;

namespace DistilledGames.States
{
    public class BuildingMode : BaseState
    {
        Building test;
        private List<Vector3Int> requiredCoords = new List<Vector3Int>();
        private Vector3Int currentSelectedCoords;

        public override void StateEnter()
        {
            base.StateEnter();
            BuildingManager.instance.ShowGrid(true);

            test = GameObject.Instantiate(BuildingManager.instance.objectTest);
            gameManager.SetBearActive(false);
            gameManager.SetItemsActive(false);
        }

        public override void StateExit()
        {
            base.StateExit();
            BuildingManager.instance.ShowGrid(false);
            gameManager.SetBearActive(true);
            gameManager.SetItemsActive(true);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            Vector3Int closestCoord = BuildingManager.instance.ClosestGridCoord();

            // What coords would be required?
            requiredCoords.Clear();
            for (int width = 0; width < test.data.Width; width++)
                for (int height = 0; height < test.data.Height; height++)
                    requiredCoords.Add(closestCoord + new Vector3Int(width, height, 0));

            // Are all these coords within bounds?
            if (BuildingManager.instance.OurCoordsWithinBounds(requiredCoords))
            {
                currentSelectedCoords = closestCoord;
                Vector3 offset = new Vector3((BuildingManager.instance.TileMapGrid.cellSize.x / 2) * test.data.Width, (BuildingManager.instance.TileMapGrid.cellSize.y / 2) * test.data.Height, 0);
                test.transform.position = BuildingManager.instance.GetWorldPosOfGridCoord(closestCoord) + offset; 
            }
        }


        public override StateDefinitions.ChangeInState PrimaryInteractionPressed()
        {
            Debug.Log("Try to place object");

            if (BuildingManager.instance.PlaceObject(new Vector2Int(currentSelectedCoords.x, currentSelectedCoords.y), test))
            {
                Debug.Log("placed");
                test = GameObject.Instantiate(BuildingManager.instance.objectTest);
            }
            else
            {
                Debug.Log("Cant place here");
            }
          

            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState MovementInput(Vector2 input)
        {
            return StateDefinitions.ChangeInState.NoChange;
        }

        public override StateDefinitions.ChangeInState EnterBuildMode()
        {
            gameManager.NextState = StateDefinitions.GameStates.Normal.ToString();
            return StateDefinitions.ChangeInState.NextState;
        }
    }
}

