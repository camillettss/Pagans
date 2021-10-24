using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Weapons/New bow")]
public class Bow : ItemBase
{
    public override void Use(Player player)
    {
        Debug.Log("bows not overload method");
    }

    public override void Use(Player player, IEntity npc, int damage = 1)
    {
        Debug.Log("BRO"+damage);
        npc.takeDamage(damage);
    }
}
