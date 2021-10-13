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
            // raccoglie
            if(player.activePlant != null)
            {
                player.activePlant.Take(player);
            }
        }
        else
        {
            // pianta
        }
    }
}
