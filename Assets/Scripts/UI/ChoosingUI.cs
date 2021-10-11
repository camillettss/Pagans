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
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            gameObject.SetActive(false);
            GameController.Instance.EnchUI.Open(slotUIs[sel].item);
        }
    }
}
