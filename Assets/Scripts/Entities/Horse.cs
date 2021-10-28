using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour, IEntity
{
    public void Interact(Player player)
    {
        player.animator.SetFloat("FacingHorizontal", GetComponent<Animator>().GetFloat("FaceX"));
        player.animator.SetFloat("FacingVertical", GetComponent<Animator>().GetFloat("FaceY"));
        player.Ride(this);
    }

    public void takeDamage(int dmg)
    {

    }

    public void ShowSignal()
    {
        // actually empty
    }
}
