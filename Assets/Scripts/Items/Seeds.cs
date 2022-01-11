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
        // pianta, set growLevels[0] at pointed position
        FindObjectOfType<GridController>().SetAt(growLevels[0], player.GetPointedPosition_vec3int());
        player.inventory.Remove(this);
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
