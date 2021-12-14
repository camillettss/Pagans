using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] PortSlotUI slotPrefab;

    List<PortSlotUI> slotUIs;
    int selected;

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }

        var prev = selected;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        if (prev != selected)
            UpdateSelection();
    }

    public void UpdateContents()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<PortSlotUI>();

        foreach(var boat in Player.i.activePort.getAccessibleBoats())
        {
            var slot = Instantiate(slotPrefab, content.transform);
            slot.boat = boat;

            slotUIs.Add(slot);
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].Text.color = GameController.Instance.unselectedDefaultColor;
            else
                slotUIs[i].Text.color = GameController.Instance.unselectedDefaultColor;
        }

        //if(slotUIs.Count>0)
        //    Player.i.cam.transform.position = slotUIs[selected].boat.transform.position;
    }
}
