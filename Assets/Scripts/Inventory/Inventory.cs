using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<InventorySlot> itemSlots;
    [SerializeField] List<InventorySlot> consumableSlots;

    List<List<InventorySlot>> allSlots;

    [Header("Entire equipment")]
    public List<InventorySlot> Rings;
    public List<InventorySlot> Weapons;
    public List<InventorySlot> Shields;
    public List<InventorySlot> Arrows;

    public Torch torch = null;

    [Header("equiped type index")]
    public int equipedWeapon = -1;
    public int equipedRing = -1;
    public int equipedShield = -1;
    public int equipedArrows = 0; // -1 stands for null

    [HideInInspector] public List<int> equips;

    public List<List<InventorySlot>> Equipment;

    public List<Rune> runes; // rune che hai trovato 
    public List<Dust> dusts;
    public List<ItemBase> gems;

    private void Awake()
    {
        allSlots = new List<List<InventorySlot>> { itemSlots, consumableSlots };
        Equipment = new List<List<InventorySlot>>() {Weapons, Arrows, Shields, Rings };
        equips = new List<int>() { equipedWeapon, equipedArrows, equipedShield, equipedRing };
    }

    public static List<string> Categories = new List<string>
    {
        "ITEMS",
        "CONSUMABLES"
    };

    public List<InventorySlot> GetSlots(int cat)
    {
        return allSlots[cat];
    }

    public InventorySlot getEquiped(int typeIndex)
    {
        // 0-> weapon, 1-> arrows, 2-> shield< 3-> ring
        if (equips[typeIndex] == -1)
            return null;
        return Equipment[typeIndex][equips[typeIndex]];
    }

    public InventorySlot getEquiped(string objname)
    {
        int ind;
        objname = objname.ToLower();
        //print($"[SYS] {objname} gonna become an :int");
        if (objname == "weapon")
            ind = 0;
        else if (objname == "arrow")
            ind = 1;
        else if (objname == "shield")
            ind = 2;
        else if (objname == "ring")
            ind = 3;
        else
            return null;

        return getEquiped(ind);
    }

    public void Equip(string itemName, int category = 0)
    {
        foreach(var item in GetSlots(category))
        {
            if (item.item.Name == itemName)
            {
                Player.i.equipedItem = item;
                return;
            }
            else
                print(item.item.Name);
        }
        GameController.Instance.ShowMessage("item not found");
    }

    public void Add(ItemBase item)
    {
        if (alreadyInStock(item))
            findItem(item).count += 1;
        else
            GetSlots(item.category).Add(new InventorySlot(item));
    }

    public void Remove(ItemBase item)
    {
        if (alreadyInStock(item))
        {
            var fitem = findItem(item); // per non chiamare troppo spesso questa funzione che potrebbe diventare troppo pesante.
            if (fitem.count > 1)
                fitem.count -= 1;
            else
                GetSlots(item.category).Remove(fitem);
        }
        else
        {
            print("bruh tf");
        }
            
    }

    bool alreadyInStock(ItemBase item)
    {
        foreach(var obj in GetSlots(item.category))
        {
            if (obj.item.Name == item.Name)
                return true;
        }
        return false;
    }

    InventorySlot findItem(ItemBase item)
    {
        foreach(var obj in GetSlots(item.category))
        {
            if (obj.item == item)
                return obj;
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemBase item;
    public int count;

    public InventorySlot(ItemBase item, int count = 1)
    {
        this.item = item;
        this.count = count;
    }
}

public class RuneContainer
{
    public List<Rune> slots = new List<Rune>(2) { null, null };

    public bool Add(int slot, Rune rune)
    {
        Debug.Log($"adding on {slot}. slots: {slots}, sel slot: {slots[slot]}");
        if(slots[slot] == null)
        {
            slots[slot] = rune;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddAndOverride(int slot, Rune rune)
    {
        slots[slot] = rune;
    }

    public int FilledSlots()
    {
        var res = 0;
        for (int i = 0; i < slots.Count; i++)
            if (slots[i] != null)
                res++;
        return res;
    }

    public List<Rune> OnlyFilledSlots()
    {
        List<Rune> res = new List<Rune>();
        
        foreach (var r in slots)
            if (r != null)
                res.Add(r);
        
        return res;
    }
}