using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : MonoBehaviour, IEntity
{
    protected bool tamed = false;
    [SerializeField] int amountToEat; // this will be randomized with a nearly number
    int ate = 0;

    public void Interact(Player player)
    {
        if (!tamed)
            nonTamedAction();
        else
            TamedAction();
    }

    public void Eat(ItemBase food)
    {
        if(food is Curative)
        {
            ate++;
            if(ate >= amountToEat)
            {
                if (!tamed)
                    tamed = true;
                // else give hps
            }
            //Player.i.inventory.Remove(food);
        }
    }

    public abstract void nonTamedAction();
    public abstract void TamedAction();

    public void takeDamage(int dmg)
    {
        // drops meat on die.
    }

    public void ShowSignal()
    {
        // actually empty
    }
}
