using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] GameObject Content;
    [SerializeField] SlotUI slotPrefab;

    [Header("Equipment Slots")]
    [SerializeField] EquipmentSlotUI Weapons;
    [SerializeField] EquipmentSlotUI Shields;
    [SerializeField] EquipmentSlotUI Rings;
    [SerializeField] EquipmentSlotUI Arrows;

    List<EquipmentSlotUI> eqUIs;

    Inventory inventory;
    List<SlotUI> slotUIs;

    int selected = 0;
    int page = 0;
    int pageSelected = 0; // scrollview

    private void Awake()
    {
        inventory = GameController.Instance.player.inventory;
        eqUIs = new List<EquipmentSlotUI>() { Weapons, Arrows, Shields, Rings };

        selected = 0;
        page = 0;

        UpdateContents();
        UpdateSelection();
    }

    public void UpdateContents()
    {
        foreach (Transform child in Content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<SlotUI>();
        foreach (var item in inventory.Equipment[selected])
        {
            var obj = Instantiate(slotPrefab, Content.transform);
            obj.SetData(item);

            slotUIs.Add(obj);
        }
        UpdateSelection();
    }

    public void HandleUpdate()
    {

        if(page == 0)
        {
            int prev = selected;

            if (Input.GetKeyDown(KeyCode.X))
            {
                gameObject.SetActive(false);
                GameController.Instance.hotbar.UpdateItems();
                GameController.Instance.state = GameState.FreeRoam;
            }

            if(Input.GetKeyDown(KeyCode.Z))
            {
                page = 1;
                UpdateSelection();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selected;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selected;

            selected = Mathf.Clamp(selected, 0, 3); // i hate hardcode but this is a static dimension, stands for Equipment Items that are four.

            if(selected != prev)
            {
                print($"prev: {prev}, selected: {selected}");
                UpdateContents();
                UpdateSelection();
            }

        }
        else
        {
            var prev = pageSelected;

            if (Input.GetKeyDown(KeyCode.X))
            {
                UpdateContents();
                page = 0;
            }
            else if(Input.GetKeyDown(KeyCode.Z))
            {
                if(inventory.equips[selected] != pageSelected)
                {
                    // Equip
                    inventory.Equip(selected, pageSelected); // servivano i puntatori
                    slotUIs[pageSelected].item.onEquip();
                    UpdateSelection();
                }
                else
                {
                    // Unequip
                    inventory.Equip(selected, -1);
                    UpdateSelection();
                }
            }
            
            if(Input.GetKeyDown(KeyCode.S))
            {
                if (inventory.equips[4] != pageSelected)
                {
                    // Equip as secondary
                    if (selected == 0)
                        inventory.Equip(4, pageSelected);
                    UpdateSelection();
                }
                else
                {
                    // Unequip
                    inventory.Equip(4, -1);
                    UpdateSelection();
                }
            }
            /*
            else if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false);
                print($"itll open with: {slotUIs[pageSelected].item.name}");
                GameController.Instance.EnchUI.Open(slotUIs[pageSelected].item);
            }
            */

            if (Input.GetKeyDown(KeyCode.UpArrow))
                --pageSelected;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                ++pageSelected;

            pageSelected = Mathf.Clamp(pageSelected, 0, slotUIs.Count-1);

            if (prev != pageSelected)
                UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        if(page == 0)
        {
            foreach (var obj in eqUIs)
                obj.SetIndicator(false);

            eqUIs[selected].SetIndicator(true);
        }
        else if(page == 1)
        {
            for(int i=0; i<slotUIs.Count; i++)
            {
                if (i == pageSelected)
                {
                    if (inventory.equips[selected] == pageSelected)
                        slotUIs[i].nameTxt.color = GameController.Instance.equipedSelectedColor;
                    else if (inventory.secondaryWeapon == pageSelected)
                        slotUIs[i].nameTxt.color = GameController.Instance.secondarySelectedColor;
                    else
                        slotUIs[i].nameTxt.color = GameController.Instance.selectedOnBookColor; // Alternativa di colore
                }

                else
                {
                    if (inventory.equips[selected] == i)
                        slotUIs[i].nameTxt.color = GameController.Instance.equipedDefaultColor;
                    else if(inventory.secondaryWeapon == i)
                        slotUIs[i].nameTxt.color = GameController.Instance.secondaryDefaultColor;
                    else
                        slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
                }
            }
        }
    }
}
