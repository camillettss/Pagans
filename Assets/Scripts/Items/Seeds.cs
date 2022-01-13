using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Seeds : ItemBase
{
    public List<Sprite> growLevels;
    public ItemBase HarvestItemDrop;

    public override void Use(Player player)
    {
        // pianta, set growLevels[0] at pointed position
        if(player.TryGetSomething(out AgribleTile tile, player.GetPointedPosition_vec2int()))
        {
            tile.Plant(this);
            player.inventory.Remove(this);
        }
    }

    public override void onEquip()
    {
        Player.i.drawSelected_Agrimap = true;
    }

    public override void onUnequip()
    {
        Player.i.drawSelected_Agrimap = false;
    }
}
