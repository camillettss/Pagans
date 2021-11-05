using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : Animal
{
    public override void nonTamedAction()
    {
        if(Player.i.inventory.Skills.Contains(tameskill))
        {
            tamed = true;
        }
    }

    public override void TamedAction()
    {
        Player.i.animator.SetFloat("FacingHorizontal", GetComponent<Animator>().GetFloat("FaceX"));
        Player.i.animator.SetFloat("FacingVertical", GetComponent<Animator>().GetFloat("FaceY"));
        Player.i.Ride(this);
    }
}
