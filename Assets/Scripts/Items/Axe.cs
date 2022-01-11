using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tools/new axe")]
public class Axe : ItemBase
{
    public override void Use(Player player)
    {
        player.animator.SetTrigger("useAxe");
        player.inventory.StartCoroutine(cut());
    }

    IEnumerator cut()
    {
        yield return new WaitForSeconds(0.3f);
        Player.i.Cut();
    }

    public override void Use(Player player, IEntity npc, int damage = 1)
    {

    }
}
