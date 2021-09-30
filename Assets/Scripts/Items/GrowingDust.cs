using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/growing dust")]
public class GrowingDust : ItemBase
{
    [SerializeField] int percentage;
    public int fortune = 0;
    public override void Use(Player player)
    {
        var collider = player.GetFrontalCollider(player.farmingLayer);
        if(collider != null)
        {
            if(Random.Range(0, 100)<percentage+fortune)
            {
                collider.GetComponent<Plant>().Grow();
            }
            else
            {
                Debug.Log("sei un fallito bro");
            }
        }
    }
}
