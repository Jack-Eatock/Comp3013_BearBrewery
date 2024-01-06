using System;
using System.Collections;
using UnityEngine;

namespace DistilledGames
{
	public enum Direction { Up, Right, Down, Left }

	[Serializable]
	public class Building : MonoBehaviour, IPlaceableObject
	{
		[HideInInspector]
		public BuildingData Data;
		private new SpriteRenderer renderer;
		protected Vector2Int gridCoords;
		private Color colourStart;

		[SerializeField] private bool isRotatable = false;
		private Direction currentRotation = Direction.Up;
		[SerializeField] private Sprite rotationUp, rotationRight, rotationDown, rotationLeft;

		[SerializeField]
		private Transform arrowsHolder;
		private IEnumerator flashingColour;
		public SpriteRenderer Rend => renderer;
		public Vector2Int GridCoords => gridCoords;

		protected virtual void Awake()
		{
			renderer = GetComponent<SpriteRenderer>();
			colourStart = renderer.color;
		}

		public void OnPlaced(Vector2Int _gridCoords)
		{
			gridCoords = _gridCoords;
		}

		private void OnDisable()
		{
			if (flashingColour != null)
			{
				StopCoroutine(flashingColour);
			}
		}

		public virtual bool Rotate(int dir)
		{
			if (!isRotatable)
			{
				return false;
			}

			int newRotation = (int)currentRotation + dir;

			currentRotation = newRotation > 3 ? 0 : newRotation < 0 ? (Direction)3 : (Direction)newRotation;

			// Do they have this rotation option?
			if (GetRotationSprite(currentRotation) == null)
			{
				return Rotate(dir);
			}

			renderer.sprite = GetRotationSprite(currentRotation);

			return true;
		}

		private Sprite GetRotationSprite(Direction dir)
		{
			return dir switch
			{
				Direction.Up => rotationUp,
				Direction.Right => rotationRight,
				Direction.Down => rotationDown,
				Direction.Left => rotationLeft,
				_ => null,
			};
		}

		public Vector2Int CordsFromDirection(Direction dir)
		{
			return dir switch
			{
				Direction.Up => new Vector2Int(0, 1),
				Direction.Right => new Vector2Int(1, 0),
				Direction.Down => new Vector2Int(0, -1),
				Direction.Left => new Vector2Int(-1, 0),
				_ => Vector2Int.zero,
			};
		}

		public void FlashColour(Color colour, float time)
		{
			if (flashingColour != null)
			{
				StopCoroutine(flashingColour);
			}

			flashingColour = FlashColourEffect(colour, time);
			_ = StartCoroutine(flashingColour);
		}

		private IEnumerator FlashColourEffect(Color colour, float time)
		{
			renderer.color = colour;
			yield return new WaitForSecondsRealtime(time);
			renderer.color = colourStart;
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
			if (arrowsHolder != null)
			{
				arrowsHolder.gameObject.SetActive(state);
			}
		}
	}
}
