using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public List<TileBase> tiles;
    public bool plowable;

    // assign in the inspector!!!
    [SerializeField] List<TileBase> TilesForStateDictGeneration;
    [SerializeField] List<PlantGrowStates> StatesForStateDictGeneration;

    [SerializeField] List<TileBase> TilesForSeedsDictGeneration;
    [SerializeField] List<Seeds> SeedsForSeedsDictGeneration;

    Dictionary<TileBase, PlantGrowStates> StateTiles;
    Dictionary<TileBase, Seeds> SeedTiles;

    void OnEnable()
    {
        if(plowable)
        {
            StateTiles = new Dictionary<TileBase, PlantGrowStates>();
            SeedTiles = new Dictionary<TileBase, Seeds>();

            for (int i = 0; i < StatesForStateDictGeneration.Count; i++)
            {
                StateTiles.Add(TilesForStateDictGeneration[i], StatesForStateDictGeneration[i]);
            }

            for (int i = 0; i < SeedsForSeedsDictGeneration.Count; i++)
            {
                SeedTiles.Add(TilesForSeedsDictGeneration[i], SeedsForSeedsDictGeneration[i]);
            }
        }
    }

    public void Interact(TileBase tile)
    {
        var AgriTile = GetPlowable(tile);
        if (AgriTile.state == PlantGrowStates.Grown)
        {
            Player.i.inventory.Add(AgriTile.seed.HarvestItemDrop);
        }
        if(AgriTile.state != PlantGrowStates.Dirt || AgriTile.state != PlantGrowStates.PlowDirt)
        {
            Player.i.inventory.Add(AgriTile.seed, Random.Range(1, 3));
        }
    }

    PlowableTileData GetPlowable(TileBase source)
    {
        var res = new PlowableTileData();

        res.seed = GetSeeds(source);
        res.state = GetState(source);

        return res;
    }

    Seeds GetSeeds(TileBase tile)
    {
        try
        {
            return SeedTiles[tile];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    // funzioni per elaborare i tile
    PlantGrowStates GetState(TileBase tile)
    {
        return StateTiles[tile];
    }
}

public class PlowableTileData
{
    public Seeds seed;
    public PlantGrowStates state;
}

public enum PlantGrowStates
{
    Dirt,
    PlowDirt,
    Seeds,
    Growing,
    Grown
}