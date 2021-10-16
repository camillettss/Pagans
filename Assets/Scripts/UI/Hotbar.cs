using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [SerializeField] Image equipedSlot; // Image interne
    [SerializeField] Image secondarySlot;

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
            equipedSlot.sprite = nothingEquiped;
        else
            equipedSlot.sprite = inventory.Weapons[inventory.equipedWeapon].item.icon;

        if (inventory.secondaryWeapon == -1)
            secondarySlot.sprite = nothingEquiped;
        else
            secondarySlot.sprite = inventory.Weapons[inventory.secondaryWeapon].item.icon;
    }
}
