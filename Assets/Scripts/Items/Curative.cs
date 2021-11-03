using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Curative")]
public class Curative : ItemBase
{
    public int cure;
    [SerializeField] bool animals_eat_this;

    public override void Use(Player player)
    {
        if(animals_eat_this)
        {
            var entity = player.GetFrontalCollider(); // animals are interactables
            if(entity.TryGetComponent(out Animal animal))
            {
                animal.Eat(this);
            }
                
        }
        else
        {
            player.hp = Mathf.Clamp(player.hp+cure, 0, player.maxHp);
        }

        player.inventory.Remove(this); // this calls the remove func, DONT CALL IT TWO TIMES!
    }
}
