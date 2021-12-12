using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchUI : MonoBehaviour
{
    [SerializeField] Image slot1;
    [SerializeField] Image slot2;
    [SerializeField] Image cost;
    [SerializeField] Image result;

    [SerializeField] Text description;

    [SerializeField] GameObject content;
    [SerializeField] RecipeTextUI textPrefab;

    List<RecipeTextUI> slotUIs;

    int selected = 0;

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        var prev = selected;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            selected++;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if (prev != selected)
            UpdateSelection();
    }

    public void UpdateContents()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        foreach(var recipe in Workbench.i.recipes)
        {
            var rUI = Instantiate(textPrefab, content.transform);
            rUI.text.text = recipe.GetData[2].Name; // result name

            slotUIs.Add(rUI);
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i = 0; i<slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].text.color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].text.color = GameController.Instance.unselectedDefaultColor;
        }

        // update triangle
        slot1.sprite = slotUIs[selected].recipe.GetData[0].icon;
        slot2.sprite = slotUIs[selected].recipe.GetData[1].icon;

        // never update cost (idk the final design)
        result.sprite = slotUIs[selected].recipe.GetData[2].icon;
    }
}
