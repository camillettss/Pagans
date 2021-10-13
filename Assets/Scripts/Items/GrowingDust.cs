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
        if(player.activePlant != null)
        {
            Debug.Log("using grow dust");
            if (Random.Range(0, 100) < percentage + fortune)
                player.activePlant.Grow();

            player.inventory.Remove(this); // Remove one
        }
    }
}
