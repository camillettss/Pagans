using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tools/growing dust")]
public class GrowingDust : ItemBase
{
    [SerializeField] int percentage;
    public int fortune = 0;

    public override void Use(Player player)
    {
        Collider2D plant = Physics2D.OverlapCircle(player.attackPoint.position, player.plantRange, player.farmingLayer);

        if(plant != null)
        {
            plant.GetComponent<Plant>().Grow();
        }
    }
}
