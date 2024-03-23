using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Random Tile", menuName = "Tiles/Random Tile")]
public class RandomTile : Tile
{
    public Sprite[] RandomTiles;
    public int weightOfFirstTile = 5; // Adjust this weight to increase the probability of the first tile

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        if (RandomTiles.Length == 0)
            return;

        // Choose a random index for the sprite with a weighted probability
        int totalWeight = weightOfFirstTile + RandomTiles.Length - 1;
        int randomWeight = Random.Range(0, totalWeight);

        int index;
        if (randomWeight < weightOfFirstTile)
        {
            // If the random weight falls within the range of the first tile's weight, select the first tile
            index = 0;
        }
        else
        {
            // Otherwise, select one of the other tiles at random
            index = Random.Range(1, RandomTiles.Length);
        }

        // Assign the weighted random sprite to the tile
        tileData.sprite = RandomTiles[index];
    }
}
