using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IEntity
{
    public void Interact(Player player)
    {
        player.transform.position = transform.position;
        player.Sleep();
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
    }
}
