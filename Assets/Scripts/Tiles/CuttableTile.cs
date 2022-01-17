using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableTile : MonoBehaviour, IEntity
{
    public void Interact(Player player)
    {
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
        Destroy(gameObject);
    }
}
