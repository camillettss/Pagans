using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    void Interact(Player player);

    void takeDamage(int dmg);

    void ShowSignal();
}
