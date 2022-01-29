using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraItemUI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] Text countText;

    public static ExtraItemUI i;

    private void Awake()
    {
        i = this;
        unShow();
    }

    public void HandleUpdate()
    {
        //print($"updating xtraUI, slot is: {Player.i.inventory.extraSlot}");
        if (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null)
            Show(Player.i.inventory.extraSlot);

        else
            unShow();
    }

    public void Show(InventorySlot item)
    {
        gameObject.SetActive(true);
        itemIcon.sprite = item.item.icon;
        countText.text = $"{item.count}";
    }

    public void unShow()
    {
        if (!gameObject.activeSelf)
            return;
        gameObject.SetActive(false);
    }
}
