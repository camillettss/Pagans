using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum HarvestingToolType { get, put };
[CreateAssetMenu(menuName ="Tools/new Harvesting tool")]
public class Harvest : ItemBase
{
    [SerializeField] HarvestingToolType type;
    [SerializeField] PlantTileData plant;

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
            var grid = FindObjectOfType<GridController>();
            var ppos = player.GetPointedPosition_vec3int();

            grid.TileAt(ppos).Interact(grid.GetTileBase(ppos));
        }
        else if(type == HarvestingToolType.put)
        {
            // plants
            var pos = player.GetPointedPosition_vec3int();
            FindObjectOfType<GridController>().Plow(pos);
        }
    }
}
