using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Curative")]
public class Curative : ItemBase
{
    public int cure;

    public override void Use(Player player)
    {
        player.hp = Mathf.Clamp(player.hp+cure, 0, player.maxHp);
        player.inventory.Remove(this);
    }
}
