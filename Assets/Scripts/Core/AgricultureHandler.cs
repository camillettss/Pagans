using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GridController))]
public class AgricultureHandler : MonoBehaviour
{
    GridController grid;

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

    void UpdateTiles()
    {
        // viene chiamata ad ogni ora del giorno

    }

    List<TileBase> RetrieveTiles() // only on startup
    {
        // calcola i tile nella griglia e applica i salvataggi
        return GetTiles();
    }
    
}
