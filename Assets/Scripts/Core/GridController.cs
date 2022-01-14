using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    Grid grid;
    [SerializeField] Tilemap hoverTilesTilemap = null;
    [SerializeField] Tilemap plowableTilemap = null;

    [SerializeField] Tile hoverTile = null;
    [SerializeField] List<TileBase> plowableTiles;

    Vector3Int oldpos = Vector3Int.zero;

    private void Start()
    {
        grid = GetComponent<Grid>();
    }

    bool pointInTilemap(Vector3Int pos)
    {
        try
        {
            if (plowableTiles.Contains(plowableTilemap.GetTile(plowableTilemap.WorldToCell(pos))))
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public void FlooshHoverTiles()
    {
        hoverTilesTilemap.ClearAllTiles();
    }

    private void Update()
    {
        if (Player.i.drawSelected_Agrimap)
        {
            var pos = Player.i.GetPointedPosition_vec3int();
            //print($"pos:{Player.i.transform.position}, pointed:{pos}");
            if (oldpos != pos)
            {
                hoverTilesTilemap.SetTile(oldpos, null);

                if (pointInTilemap(pos))
                    hoverTile.color = Color.green;
                else
                    hoverTile.color = Color.red;

                hoverTilesTilemap.SetTile(pos, hoverTile);

                oldpos = pos;
            }
        }
    }
}
