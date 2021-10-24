using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName ="Runes/new generic rune")]
public class Rune : ItemBase
{
    public int longDmgAmount = 0;
    public int closeDmgAmount = 0;
    public int DefenseAmount = 0;

    public string Letter;

    [SerializeField] protected List<Skill> skillsRequired;

    public override void Use(Player player)
    {
        
    }

    public void skillCheck(Func<int> runlater)
    {
        foreach (var skill in skillsRequired)
            if (!Player.i.inventory.Skills.Contains(skill))
                return;

        runlater();
    }
}