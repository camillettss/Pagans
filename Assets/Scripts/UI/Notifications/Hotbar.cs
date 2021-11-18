using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [SerializeField] Image equipedSlot; // Image interne
    [SerializeField] Image secondarySlot;
    [SerializeField] Image itemSlot;
    [SerializeField] Image shieldSlot;

    [SerializeField] Text weaponTip;
    [SerializeField] Text equipedTip;
    [SerializeField] Text shieldTip;

    [SerializeField] Sprite nothingEquiped;

    Inventory inventory;

    private void Start()
    {
        inventory = Player.i.inventory;
        UpdateItems();
    }

    public void UpdateItems()
    {
        if (inventory.equipedWeapon == -1)
        {
            equipedSlot.sprite = nothingEquiped;
            weaponTip.text = "";
        }
        else
        {
            equipedSlot.sprite = inventory.Weapons[inventory.equipedWeapon].item.icon;
            weaponTip.text = "R";
        }

        if (inventory.secondaryWeapon == -1)
            secondarySlot.sprite = nothingEquiped;
        else
            secondarySlot.sprite = inventory.Weapons[inventory.secondaryWeapon].item.icon;

        if (inventory.equipedTool == -1)
        {
            itemSlot.sprite = nothingEquiped;
            equipedTip.text = "";
        }
        else
        {
            itemSlot.sprite = inventory.Tools[inventory.equipedTool].item.icon;
            equipedTip.text = "E";
        }

        if (inventory.equipedShield == -1)
        {
            shieldSlot.sprite = nothingEquiped;
            shieldTip.text = "";
        }
        else
        {
            shieldSlot.sprite = inventory.Shields[inventory.equipedShield].item.icon;
            shieldTip.text = "X";
        }
    }
}
