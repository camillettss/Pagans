using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ItemBase : ScriptableObject
{
    public string Name;
    public string description;
    public string presentationDesc = "a new item.";
    public int price = 5;
    public Sprite icon;

    public bool discovered = false;

    [Header("Equipment")]
    public int closeDamage;
    public int longDamage;
    public int Defense;

    public Dialogue presentationDialogue;

    public Dust dust;

    public RuneContainer runes = new RuneContainer();

    // categories: 0 -> item, 1 -> consumable, -1 -> magic item, -2 -> equipment/weapon, -3 equipment/arrow

    [SerializeField] protected int Category;
    public virtual int category { get => Category; }

    #region inventory stuffs
    public virtual void onEquip()
    {

    }
    public virtual void onUnequip()
    {

    }
    #endregion

    public abstract void Use(Player player);

    public virtual void Use(Player player, IEntity npc, int damage = 1)
    {

    }
    
    public int GetLongDamage() // Per le rune
    {
        int res = longDamage;
        foreach (var rune in runes.OnlyFilledSlots())
        {
            Debug.Log(rune);
            res += rune.longDmgAmount;
        }
        return res;
    }

    public int GetCloseDamage()
    {
        int res = closeDamage;
        foreach (var rune in runes.OnlyFilledSlots())
            res += rune.closeDmgAmount;
        return res;
    }

    public int GetDefense()
    {
        return Defense;
    }

}