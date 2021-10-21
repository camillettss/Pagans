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
    public List<InventorySlot> Tools;

    public Torch torch = null;

    [Header("equiped type index")]
    public int equipedWeapon = -1;
    public int secondaryWeapon = -1;
    public int equipedRing = -1;
    public int equipedShield = -1;
    public int equipedTool = 0; // -1 stands for null

    public List<int> equips;
    
    public List<List<InventorySlot>> Equipment;

    public List<Rune> runes; // rune che hai trovato 
    public List<Dust> dusts;
    public List<ItemBase> gems;

    private void Awake()
    {
        //                                            0           1
        allSlots = new List<List<InventorySlot>> { itemSlots, consumableSlots };
        //                                             -1       -2     -3      -4
        Equipment = new List<List<InventorySlot>>() {Weapons, Tools, Shields, Rings };
        updateEquipsList(); // dovrebbe essere una lista di puntatori..
        // wp, tool, shield, ring, secondary
    }

    public static List<string> Categories = new List<string>
    {
        "ITEMS",
        "CONSUMABLES"
    };

    void updateEquipsList()
    {
        equips = new List<int>() { equipedWeapon, equipedTool, equipedShield, equipedRing, secondaryWeapon };
    }

    public List<InventorySlot> GetSlots(int cat)
    {
        if (cat == 0 || cat == 1)
            return allSlots[cat];
        else if (cat == -1)
            return Weapons;
        else if (cat == -2)
            return Tools;
        else if (cat == -3)
            return Shields;
        else if (cat == -4)
            return Rings;
        else
            return new List<InventorySlot>();
    }

    public bool isThereObjectOfType<T>(int type)
    {
        foreach(var item in GetSlots(type))
        {
            if (item.item is T)
                return true;
        }
        return false;
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
            ind = -1;
        else if (objname == "arrow")
            ind = -2;
        else if (objname == "shield")
            ind = -3;
        else if (objname == "ring")
            ind = -4;
        else
            return null;

        return getEquiped(ind);
    }

    public void Equip(int itype, int val) // questo deve fare riferimento alla UI
    {
        if (itype == 4)
            print("lesgoo");

        if (itype == 0)
            equipedWeapon = val;
        else if (itype == 1)
            equipedTool = val;
        else if (itype == 2)
            equipedShield = val;
        else if (itype == 3)
            equipedRing = val;
        else if (itype == 4)
            secondaryWeapon = val;
        else
            return;
        

        updateEquipsList();
    }

    public void Add(ItemBase item)
    {
        if(!item.discovered && !GameController.Instance.newItemUI.isActiveAndEnabled) // altrimenti se prende due oggetti fa un casino della madonna
        {
            GameController.Instance.newItemUI.Open(item);
            item.discovered = true;
        }
        if (alreadyInStock(item))
            findItem(item).count += 1;
        else
        {
            if(item.category == 0 || item.category == 1) // item or consumable
                GetSlots(item.category).Add(new InventorySlot(item));

            else if(item.category < 0) // is an equipment item
            {
                if(item.category == -1) // weapon
                {
                    Weapons.Add(new InventorySlot(item));
                }
                if (item.category == -2) // tool or consumableTool
                {
                    Tools.Add(new InventorySlot(item));
                }
                if (item.category == -3) // shield
                {
                    Shields.Add(new InventorySlot(item));
                }
                if (item.category == -4) // ring
                {
                    Rings.Add(new InventorySlot(item));
                }
            }

            else
            {
                if(item.category == 2) // 2 are runes
                {
                    print("Adding a rune");
                    runes.Add((Rune)item);
                }
                else if(item.category == 3) // 3 are dusts
                {
                    dusts.Add((Dust)item);
                }

            }
        }
        NotificationsUI.i.AddNotification($"took {item.Name}");
        updateEquipsList();
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

        updateEquipsList();
            
    }

    public void RemoveAt(int cat, int sel)
    {
        GetSlots(cat).RemoveAt(sel);
        updateEquipsList();
    }

    public bool alreadyInStock(ItemBase item)
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