using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FloorTile : Tile
{
    public Sprite[] FloorTiles;
    private Sprite sprite;
    private bool spriteSet = false;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (!spriteSet)
            sprite = FloorTiles[Random.Range(0, FloorTiles.Length)];
        tileData.sprite = sprite;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/CustomTiles/FloorTile")]
    public static void CreateFloorTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Floor Tile", "New Floor Tile", "Asset", "Save Floor Tile", "Assets");
        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FloorTile>(), path);
    }
#endif
}
