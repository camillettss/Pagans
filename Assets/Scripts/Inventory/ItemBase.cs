using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using UnityEngine.Localization;

public abstract class ItemBase : ScriptableObject
{
    [SerializeField] private LocalizedString _Name;
    public string Name
    {
        get => _Name.GetLocalizedString();
        private set
        {

        }
    }
    [SerializeField] private LocalizedString _description;
    public string description
    {
        get => _description.GetLocalizedString();
        private set
        {

        }
    }
    [SerializeField] private LocalizedString _presentationDesc;
    public string presentationDesc
    {
        get => _presentationDesc.GetLocalizedString();
        private set
        {

        }
    }
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

    public ItemBase cookedResult = null;
    public int cookTime;

    public bool HasDerivated = false;
    [SerializeField] bool harvestingItem = false;

    public Sprite droppedSprite;

    [ConditionalField(nameof(HasDerivated))] public bool hasItemCost = false;
    [ConditionalField(nameof(HasDerivated))] public ItemBase handcraftDerivatedItem;
    [ConditionalField(nameof(HasDerivated))] public int craftResultQuantity=1;
    [ConditionalField(nameof(HasDerivated))] public ItemBase craftCost;
    [ConditionalField(nameof(HasDerivated))] public int craftCostCount;
    [ConditionalField(nameof(HasDerivated))] public int craftExperienceReward;
    [ConditionalField(nameof(HasDerivated))] public Sprite craftMethodIcon;

    // categories: 0 -> item, 1 -> consumable, -1 -> magic item, -2 -> equipment/weapon, -3 equipment/arrow

    [SerializeField] protected int Category;
    public virtual int category { get => Category; }

    #region inventory stuffs
    public virtual void onEquip()
    {

    }
    public virtual void onUnequip()
    {
        if (harvestingItem)
        {
            try
            {
                FindObjectOfType<GridController>().FlooshHoverTiles();
            }
            catch
            {

            }
        }
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

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip()
    {

    }

}

public class TypeMaintainer<T>: ScriptableObject where T: ScriptableObject
{
    public T original;
}