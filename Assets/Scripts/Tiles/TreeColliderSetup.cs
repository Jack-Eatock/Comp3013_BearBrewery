using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TreeBottomCollider : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        AddCollidersToBottomTiles();
    }

    void AddCollidersToBottomTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    // Check if the tile below is empty to ensure it's the bottom tile of a tree
                    TileBase tileBelow = tilemap.GetTile(new Vector3Int(x, y - 1, 0));
                    if (tileBelow == null)
                    {
                        Vector3Int cellPosition = new Vector3Int(x, y, 0);
                        Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
                        AddCollider(worldPosition);
                    }
                }
            }
        }
    }

    void AddCollider(Vector3 position)
    {
        GameObject colliderGameObject = new GameObject("TreeCollider");
        colliderGameObject.transform.parent = this.transform;
        colliderGameObject.transform.position = position + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);

        // Add a BoxCollider2D and configure it as needed
        BoxCollider2D collider = colliderGameObject.AddComponent<BoxCollider2D>();
        // Set the size of the collider to match your tile size
        collider.size = new Vector2(tilemap.cellSize.x, tilemap.cellSize.y);
        // Adjust the collider offset to place it at the bottom of the tile
        collider.offset = new Vector2(0, -tilemap.cellSize.y / 2);
    }
}
