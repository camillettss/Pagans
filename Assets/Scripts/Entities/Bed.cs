using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IEntity
{
    public void Interact(Player player)
    {
        if (GameController.Instance.hours >= 20 || GameController.Instance.hours <= 5)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            player.transform.position = transform.position;
            player.Sleep(() => { onWakeUp(); });
        }
    }

    void onWakeUp()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
    }
}
