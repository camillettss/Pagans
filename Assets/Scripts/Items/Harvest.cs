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
            var collider = player.GetFrontalCollider(player.farmingLayer);
            if(collider != null)
            {
                collider.GetComponent<Plant>().Take(player);
            }
        }
        else
        {
            // pianta
        }
    }
}
