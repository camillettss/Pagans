using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTile : Tile
{
    [SerializeField] List<Sprite> growing_levels;

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/PlantTile")]
    public static void CreatePlantTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Plant Tile", "New Plant Tile", "Asset", "Save Plant Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PlantTile>(), path);
    }
#endif
}
