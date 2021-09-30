using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    public UnityEngine.UI.Text nameTxt;
    public ItemBase item;
    public int price;

    public void SetData(InventorySlot slot, int price = 0)
    {
        item = slot.item;
        nameTxt.text = item.Name;
        this.price = price;
    }
}
