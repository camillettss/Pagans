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
    }

    public void HandleUpdate()
    {

        if(page == 0)
        {
            int prev = selected;

            if (Input.GetKeyDown(KeyCode.X))
            {
                gameObject.SetActive(false);
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
                // Equip
                inventory.Equip(selected, pageSelected); // servivano i puntatori
                slotUIs[pageSelected].item.onEquip();
                print($"setted: {inventory.equips[selected]} to {pageSelected}");

                /*
                 * inventory.equips[selected] = pageSelected;
                // set equips[index] al valore corrispondente ad item IN INVENTORY.EQUIPMENT, equips tiene l'indice dentro le sublists di equipment.
                print($"setted equips[] to {pageSelected}, that matches with: {inventory.Equipment[selected][pageSelected].item.Name}");
                // now update hotbars data
                GameController.Instance.hotbar.UpdateItems();
                 */
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
                    slotUIs[i].nameTxt.color = GameController.Instance.selectedDefaultColor;
                else
                    slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }
    }
}
