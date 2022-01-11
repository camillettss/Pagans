using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Seeds : ItemBase
{
    public List<UnityEngine.Tilemaps.TileBase> growLevels;
    public ItemBase HarvestItemDrop;

    public override void Use(Player player)
    {
        
    }
}
