using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public List<TileBase> tiles;
    public bool plowable;

    [SerializeField] Dictionary<TileBase, PlantGrowStates> visualStateTiles = new Dictionary<TileBase, PlantGrowStates>();

    public void Interact(TileBase found)
    {
        Debug.Log($"state:{GetState(found)}");
    }

    public PlantGrowStates GetState(TileBase tile)
    {
        return PlantGrowStates.Dirt;
    }

}

public enum PlantGrowStates
{
    Dirt,
    PlowDirt,
    Seeds,
    Growing,
    Grown
}