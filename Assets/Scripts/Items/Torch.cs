using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/new torch")]
public class Torch : ItemBase
{
    public float brightness = 0.3f;
    public bool bright = false;

    public float radius = 0.5f;

    public override void Use(Player player)
    {
        bright = !bright;
    }

    public override void Equip(InventorySlot item)
    {
        GameController.Instance.player.inventory.torch = (Torch)item.item;
    }
}