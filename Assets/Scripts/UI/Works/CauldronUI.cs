using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CauldronUI : MonoBehaviour
{
    [SerializeField] GameObject content; // scrollview
    [SerializeField] Image itemIcon;
    [SerializeField] GameObject TextPrefab;

    int selected = 0;
    List<Text> slotUIs;

    private void Awake()
    {
        UpdateContents();
    }

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

        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if(selected != prev)
        {
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Perform();
        }
    }

    public void UpdateContents()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<Text>();

        foreach(var consumable in Player.i.inventory.GetByBookmark(2)[0])
        {
            var t = Instantiate(TextPrefab, content.transform);
            t.GetComponent<Text>().text = consumable.item.Name;

            slotUIs.Add(t.GetComponent<Text>());
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].color = GameController.Instance.unselectedDefaultColor;
        }
    }

    void Perform()
    {
        print($"using: {slotUIs[selected].text}.");
    }

}
