using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] ChoosableItemUI choosablePrefab;
    List<ChoosableItemUI> slotUIs = new List<ChoosableItemUI>();

    int sel = 0;

    private void Awake()
    {
        sel = 0;
    }

    public void UpdateItems()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<ChoosableItemUI>();
        foreach(var wp in Player.i.inventory.Weapons)
        {
            var obj = Instantiate(choosablePrefab, content.transform);
            obj.text.text = wp.item.Name;
            obj.item = wp.item;

            slotUIs.Add(obj);
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == sel)
                slotUIs[i].text.color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].text.color = GameController.Instance.unselectedDefaultColor;
        }
    }

    public void HandleUpdate()
    {
        /*var prev = sel;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            sel++;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --sel;

        sel = Mathf.Clamp(sel, 0, slotUIs.Count-1);

        if (prev != sel)
            UpdateSelection();

        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        else if(Input.GetKeyDown(KeyCode.Z))
        {
            gameObject.SetActive(false);
            GameController.Instance.EnchUI.Open(slotUIs[sel].item);
        }*/
    }
}
