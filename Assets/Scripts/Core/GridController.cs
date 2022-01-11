using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    Grid grid;
    [SerializeField] Tilemap hoverTilesTilemap = null;
    public Tilemap plowableTilemap;
    [SerializeField] Tile hoverTile = null;
    public Tile plowTile;
    [SerializeField] List<TileData> tileDatas;
    Dictionary<TileBase, TileData> dataFromTiles;

    Vector3Int oldpos = Vector3Int.zero;

    private void Start()
    {
        grid = GetComponent<Grid>();

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach(TileData tileData in tileDatas)
        {
            foreach(TileBase tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public void Plow(Vector3Int pos)
    {
        plowableTilemap.SetTile(plowableTilemap.WorldToCell(pos), plowTile);
    }

    public TileBase GetTileBase(Vector3 pos)
    {
        return plowableTilemap.GetTile(plowableTilemap.WorldToCell(pos));
    }

    TileData GetTileData(TileBase tilebase)
    {
        if (tilebase != null)
            return dataFromTiles[tilebase];
        else
            return null;
    }

    public void SetAt(TileBase tile, Vector3 pos)
    {
        plowableTilemap.SetTile(plowableTilemap.WorldToCell(pos), tile);
    }

    public TileData TileAt(Vector3 pos)
    {
        return GetTileData(GetTileBase(pos));
    }

    public bool TryGetTileAt(Vector3 pos, out TileData data)
    {
        data = TileAt(pos);
        if (data != null)
            return true;
        else return false;
    }

    bool pointInTilemap(Vector3Int pos)
    {
        try
        {
            if (TileAt(pos).plowable)
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
