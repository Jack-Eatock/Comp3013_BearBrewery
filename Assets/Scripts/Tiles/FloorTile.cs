using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FloorTile : Tile
{
	public Sprite[] FloorTiles;
	private new Sprite sprite;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap)
	{
		base.RefreshTile(position, tilemap);
	}

	private int CalculateIndexForPosition(Vector3Int position)
	{
		// Pattern:
		int[] pattern = { 0, 1, 1, 2, 1, 1 };
		int patternLength = pattern.Length;

		// Since the top row should start with 1, we offset the x position by 1
		int correctedX = (position.x + 1) % patternLength;

		// Adjust for the shift in each row.
		int indexInPattern = (correctedX - position.y) % patternLength;

		// Make sure we get a positive index
		if (indexInPattern < 0)
		{
			indexInPattern += patternLength;
		}

		// Get the actual index from the pattern
		int index = pattern[indexInPattern];

		// 10% chance to change index '1' to '3' or '4'
		if (index == 1 && Random.Range(0, 10) < 1) // 10% chance
		{
			index = Random.Range(3, 5); // Randomly returns 3 or 4
		}

		return index;
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		base.GetTileData(position, tilemap, ref tileData);

		// Calculate the index based on position to create the pattern
		int index = CalculateIndexForPosition(position);

		// Assign the calculated sprite index
		sprite = FloorTiles[index];
		tileData.sprite = sprite;
	}

#if UNITY_EDITOR
	[MenuItem("Assets/Create/2D/CustomTiles/FloorTile")]
	public static void CreateFloorTile()
	{
		string path = EditorUtility.SaveFilePanelInProject("Save Floor Tile", "New Floor Tile", "Asset", "Save Floor Tile", "Assets");
		if (path == "")
		{
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FloorTile>(), path);
	}
#endif
}
