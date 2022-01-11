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
        StartCoroutine(SlowUpdate());
    }

    IEnumerator SlowUpdate()
        // called every 5 seconds
    {
        while (true)
        {
            UpdateTiles();
            yield return new WaitForSeconds(5f);
        }
    }

    List<TileBase> GetTiles()
    {
        var res = new List<TileBase> (grid.plowableTilemap.GetTilesBlock(grid.plowableTilemap.cellBounds));
        res.RemoveAll(x => x == null);
        return res;
    }

    void UpdateTiles()
    {
        // aggiornale in base al tempo
    }
    
}
