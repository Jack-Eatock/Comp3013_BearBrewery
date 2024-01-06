using UnityEngine;

namespace DistilledGames
{
	public static class Helper
	{
		/// <summary>
		/// Ensures the bear is in the correct layer, to give the ilusion of the game being 3D
		/// </summary>
		public static void UpdateSortingOrder(SpriteRenderer _rend, Transform _transform, int offset = 0)
		{
			float exactYPos = _transform.position.y;
			exactYPos *= 10f;
			_rend.sortingOrder = 500 - Mathf.RoundToInt(exactYPos) + offset;
		}
	}
}

