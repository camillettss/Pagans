using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName ="Items/new rune")]
public class Rune : ItemBase
{
    public int longDmgAmount = 0;
    public int closeDmgAmount = 0;
    public int DefenseAmount = 0;

    public string Letter;

    private void Awake()
    {
        category = -1;
    }

    public override void Use(Player player)
    {
        
    }
}
