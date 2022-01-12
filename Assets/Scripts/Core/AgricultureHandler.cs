using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GridController))]
public class AgricultureHandler : MonoBehaviour
{
    GridController grid;

    List<AgriTile> Tiles;

    private void Awake()
    {
        grid = GetComponent<GridController>();
        grid.plowableTilemap.CompressBounds();
    }

    public void HandleUpdate() // called on hour change
    {
        UpdateTiles();
    }

    List<TileBase> GetTiles()
    {
        var res = new List<TileBase> (grid.plowableTilemap.GetTilesBlock(grid.plowableTilemap.cellBounds));
        res.RemoveAll(x => x == null);
        return res;
    }

    void UpdateTiles() // se c'è una data completata cambia sprite
    {
        foreach(var tile in Tiles)
        {
            var calc_tbase = tile.CalcTileBase();
            if(calc_tbase != tile.tilebase) // cresciuto
            {
                grid.plowableTilemap.SetTile(tile.pos, calc_tbase); // update the tile
                tile.tilebase = calc_tbase;
            }
        }
    }

    void SetupTiles() // only on startup
    {
        // calcola i tile nella griglia e applica i salvataggi
        foreach(var tile in GetTiles())
        {
            break;
        }
    }
    
}

public class AgriTile // basically a tile data container
{
    public Vector3Int pos; // *in-grid* position
    public Date date; // data di fine crescita

    public TileBase tilebase;
    public TileData tiledata; // tile type (sempre plowable, serve per il riferimento.

    public AgriTile(Vector3Int pos, Date date, TileBase tbase, TileData tdata)
    {
        this.pos = pos;
        this.date = date;
        tilebase = tbase;
        tiledata = tdata;
    }

    public TileBase CalcTileBase()
    {
        return null;
    }
}