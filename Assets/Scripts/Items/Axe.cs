using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tools/new axe")]
public class Axe : ItemBase
{
    public override void Use(Player player)
    {
        Debug.Log("calling player.cut");
        player.Cut();
    }

    public override void Use(Player player, IEntity npc, int damage = 1)
    {

    }
}
