using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject scrollviewContent;
    [SerializeField] SlotUI slotPrefab;

    [SerializeField] UnityEngine.UI.Text DescriptionTxt;
    [SerializeField] UnityEngine.UI.Text CategoryTxt;
    [SerializeField] UnityEngine.UI.Image Icon;
    [SerializeField] Sprite empty_icon;

    List<SlotUI> slotUIs;

    Inventory inventory;

    int selectedItm = 0;
    int selectedCat = 0;

    private void Awake()
    {
        inventory = FindObjectOfType<Player>().inventory;
        UpdateContents();
    }

    public void UpdateContents()
    {
        foreach (Transform child in scrollviewContent.transform)
            Destroy(child.gameObject);

        CategoryTxt.text = Inventory.Categories[selectedCat];

        if(inventory.GetSlots(selectedCat).Count <= 0)
        {
            DescriptionTxt.text = "no items here";
            Icon.sprite = empty_icon;
            return;
        }

        slotUIs = new List<SlotUI>();
        foreach(var item in inventory.GetSlots(selectedCat))
        {
            var slot = Instantiate(slotPrefab, scrollviewContent.transform);
            slot.SetData(item);
            slotUIs.Add(slot);
        }

        selectedItm = 0;

        try
        {
            UpdateSelection();
        }
        catch
        {

        }
        
    }

    public void UpdateSelection()
    {
        for(int i=0; i < slotUIs.Count; i++)
        {
            if(i == selectedItm)
            {
                slotUIs[i].nameTxt.color = GameController.Instance.selectedOnBookColor;
            }
            else
            {
                slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }

        DescriptionTxt.text = inventory.GetSlots(selectedCat)[selectedItm].item.description;
        Icon.sprite = inventory.GetSlots(selectedCat)[selectedItm].item.icon;
    }

    public void HandleUpdate()
    {
        /*int prevSel = selectedItm;
        int prevCat = selectedCat;

        if(Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItm;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItm;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedCat;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedCat;

        if (selectedCat < 0)
            selectedCat = Inventory.Categories.Count - 1;
        else if(selectedCat > Inventory.Categories.Count-1)
            selectedCat = 0;

        selectedItm = Mathf.Clamp(selectedItm, 0, inventory.GetSlots(selectedCat).Count-1);

        if (prevCat != selectedCat)
            UpdateContents();
        else if (prevSel != selectedItm && inventory.GetSlots(selectedCat).Count > 0)
            UpdateSelection();

        /*if (Input.GetKeyDown(KeyCode.Z)) // Equip
        {
            var item = inventory.GetSlots(selectedCat)[selectedItm];
            item.item.Equip(item);
            GameController.Instance.hotbar.UpdateItems();
        }*/

    }
}
