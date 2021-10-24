using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Skills/book")]
public class Book : ItemBase
{
    public Skill skillToLearn;
    public int learningCost;

    public override void Use(Player player)
    {
        if(player.experience >= learningCost)
        {
            player.experience -= learningCost;
            player.inventory.Learn(this);
        }
    }
}
