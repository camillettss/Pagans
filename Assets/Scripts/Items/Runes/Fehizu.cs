using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Runes/Fehizu")]
public class Fehizu : Rune
{
    public override void Use(Player player)
    {
        skillCheck(function);
    }

    public int function()
    {
        Debug.Log("why are you using such powerful weapon??");
        return 0;
    }
}