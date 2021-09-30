using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public List<Image> itemSlots;
    Player player;

    [SerializeField] Sprite torchBG;
    [SerializeField] Sprite weaponBG;
    [SerializeField] Sprite equipedBG;
    [SerializeField] Sprite arrowBG;

    private void Start()
    {
        player = GameController.Instance.player;
    }

    public void UpdateItems()
    {
        if (player.inventory.torch != null)
            itemSlots[0].sprite = player.inventory.torch.icon;
        else
            itemSlots[0].sprite = torchBG;

        if (player.equipedItem != null && player.equipedItem.item != null)
            itemSlots[1].sprite = player.equipedItem.item.icon;
        else
            itemSlots[1].sprite = equipedBG;

        if (player.inventory.getEquiped("weapon") != null)
            itemSlots[2].sprite = player.inventory.getEquiped("weapon").item.icon;
        else
            itemSlots[2].sprite = weaponBG;

        if (player.inventory.getEquiped("arrow") != null)
            itemSlots[3].sprite = player.inventory.getEquiped("arrow").item.icon;
        else
            itemSlots[3].sprite = arrowBG;

    }
}
