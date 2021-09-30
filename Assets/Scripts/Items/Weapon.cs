using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Weapon")]
public class Weapon : ItemBase
{
    public override void Use(Player player)
    {
        Debug.Log($"{name}'s use() non ha ricevuto i parametri necessari quindi non è stato possibile chiamare l'overload.");
    }

    public override void Use(Player player, NPCController npc, int damage = 1)
    {
        npc.takeDamage(damage);
        if (dust != null)
            dust.onHit();
    }
}
