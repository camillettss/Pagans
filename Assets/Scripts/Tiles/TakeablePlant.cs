using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeablePlant : MonoBehaviour, IEntity
{
    [SerializeField] ItemBase drop;
    public void Interact(Player player)
    {
        player.inventory.Add(drop);
        Destroy(gameObject);
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
    }
}
