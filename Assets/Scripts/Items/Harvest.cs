using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HarvestingToolType { get, put };
[CreateAssetMenu(menuName ="Items/new Harvesting tool")]
public class Harvest : ItemBase
{
    [SerializeField] HarvestingToolType type;

    public override void Use(Player player)
    {
        if(type == HarvestingToolType.get)
        {
            Collider2D plant = Physics2D.OverlapCircle(player.attackPoint.position, player.plantRange, player.farmingLayer);

            if (plant != null)
            {
                plant.GetComponent<Plant>().Take(player);
            }
        }
        else
        {
            // pianta
        }
    }
}
