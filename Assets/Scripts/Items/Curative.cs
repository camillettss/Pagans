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
        if(animals_eat_this && ThereIsAnimal(out IAnimal animal))
        {
            animal.Eat(this);
            player.inventory.Remove(this);
        }
        else
        {
            player.hp = Mathf.Clamp(player.hp+cure, 0, player.maxHp);
            player.inventory.Remove(this);
        }
    }

    bool ThereIsAnimal(out IAnimal animal)
    {
        animal = null;

        var entity = Player.i.GetFrontalCollider();
        if (entity != null && entity.TryGetComponent(out IAnimal m_animal))
        {
            animal = m_animal;
            return true;
        }
        else return false;
    }
}
