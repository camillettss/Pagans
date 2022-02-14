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

    [SerializeField] Text cost1;
    [SerializeField] Text cost2;

    List<RecipeTextUI> slotUIs;

    int selected = 0;

    bool cost1Affordable = true;
    bool cost2Affordable = true;

    CraftingType actualType;

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
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

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (cost1Affordable && cost2Affordable)
            {
                if (Player.i.activeBench.craftingType == CraftingType.Table)
                    Workbench.i.Craft(slotUIs[selected].recipe);

                else if(Player.i.activeBench.craftingType == CraftingType.Naval)
                    Workbench.i.Spawn(slotUIs[selected].recipe.boat_result, Player.i.activeBench.objectSpawner);

                UpdateSelection();
            }
        }*/
    }

    public void UpdateContents()
    {
        actualType = Player.i.activeBench.craftingType;

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<RecipeTextUI>();

        foreach(var recipe in Workbench.i.GetRecipes(Player.i.activeBench.craftingType))
        {
            var rUI = Instantiate(textPrefab, content.transform);
            rUI.recipe = recipe;

            if(actualType == CraftingType.Table)
                rUI.text.text = recipe.result.Name; // result name
            else
                rUI.text.text = recipe.boat_result.Name; // boat name

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

        if(slotUIs.Count > 0)
        {
            // update triangle
            slot1.sprite = slotUIs[selected].recipe.slot1.icon;
            slot2.sprite = slotUIs[selected].recipe.slot2.icon;

            // never update cost (idk the final design)
            if(actualType == CraftingType.Table)
                result.sprite = slotUIs[selected].recipe.result.icon;
            else
                result.sprite = slotUIs[selected].recipe.boat_result.icon;

            checkCost();

            cost1.text = $"1 {slotUIs[selected].recipe.slot1.Name}";
            cost2.text = $"1 {slotUIs[selected].recipe.slot2.Name}";

            if (cost1Affordable)
                cost1.color = GameController.Instance.AffordableGreenColor;
            else
                cost1.color = GameController.Instance.UnaffordableRedColor;

            if (cost2Affordable)
                cost2.color = GameController.Instance.AffordableGreenColor;
            else
                cost2.color = GameController.Instance.UnaffordableRedColor;
        }

    }

    void checkCost()
    {
        if (Player.i.inventory.alreadyInStock(slotUIs[selected].recipe.slot1))
        {
            cost1Affordable = true;
        }
        else cost1Affordable = false;
        if (Player.i.inventory.alreadyInStock(slotUIs[selected].recipe.slot2))
        {
            cost2Affordable = true;
        }
        else
            cost2Affordable = false;
    }
}
