using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum HarvestingToolType { get, put };
[CreateAssetMenu(menuName ="Tools/new Harvesting tool")]
public class Harvest : ItemBase
{
    [SerializeField] HarvestingToolType type;

    public override void onEquip()
    {
        Player.i.drawSelected_Agrimap = true;
    }

    public override void onUnequip()
    {
        Player.i.drawSelected_Agrimap = false;
    }

    public override void Use(Player player)
    {
        if (type == HarvestingToolType.get)
        {
            if (player.TryGetSomething(out AgribleTile tile, player.GetPointedPosition_vec3int()))
            {
                if(tile.seed != null)
                {
                    if (tile.isGrown)
                    {
                        Player.i.inventory.Add(tile.seed.HarvestItemDrop);
                    }
                    Player.i.inventory.Add(tile.seed, Random.Range(1, 3));
                }
            }
        }
        else if(type == HarvestingToolType.put)
        {
            // plows a tile
            if(player.TryGetSomething(out AgribleTile tile, player.GetPointedPosition_vec3int()))
            {
                tile.Plow();
            }
            
        }
    }
}
