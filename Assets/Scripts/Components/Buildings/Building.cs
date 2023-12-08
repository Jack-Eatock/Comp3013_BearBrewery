using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DistilledGames
{
    public enum Direction { Up, Right, Down, Left }

    [Serializable]
    public class Building : MonoBehaviour, IPlaceableObject
    {
        [HideInInspector]
        public BuildingData data;
        private SpriteRenderer renderer;
        protected Vector2Int gridCoords;

        [SerializeField] private bool isRotatable = false;
        private Direction currentRotation = Direction.Up;
        [SerializeField] private Sprite rotationUp, rotationRight, rotationDown, rotationLeft;

        [SerializeField]
        private Transform arrowsHolder;

        public SpriteRenderer Rend => renderer;
        public Vector2Int GridCoords => gridCoords; 

        protected virtual void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void OnPlaced(Vector2Int _gridCoords)
        {
            gridCoords = _gridCoords;
        }

        public virtual bool Rotate(int dir)
        {
            if (!isRotatable)
                return false;

            int newRotation = (int)currentRotation + dir;

            if (newRotation > 3)
                currentRotation = (Direction) 0;
            else if (newRotation < 0)
                currentRotation = (Direction) 3;
            else
                currentRotation = (Direction) newRotation;

            // Do they have this rotation option?
            if (GetRotationSprite(currentRotation) == null)
                return Rotate(dir);
             
            renderer.sprite = GetRotationSprite(currentRotation);

            return true;
        }

        private Sprite GetRotationSprite(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return rotationUp;

                case Direction.Right:
                    return rotationRight;

                case Direction.Down:
                    return rotationDown;

                case Direction.Left:
                    return rotationLeft;
            }
            return null;
        }

        public Vector2Int CordsFromDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new Vector2Int(0,1);

                case Direction.Right:
                    return new Vector2Int(1, 0);

                case Direction.Down:
                    return new Vector2Int(0, -1); 

                case Direction.Left:
                    return new Vector2Int(-1, 0);
            }
            return Vector2Int.zero;
        }

        public virtual bool SetRotation(Direction index)
        {
            if (isRotatable) 
            { 
                currentRotation = index;
                renderer.sprite = GetRotationSprite(currentRotation);
                return true;
            }
            return false;
        }

        public virtual Direction GetDirection()
        {
            return currentRotation;
        }

        public virtual void OnDeleted()
        {
           
        }

        public void ShowArrows(bool state)
        {
            if(arrowsHolder != null)
                arrowsHolder?.gameObject.SetActive(state);
        }
    }
}
