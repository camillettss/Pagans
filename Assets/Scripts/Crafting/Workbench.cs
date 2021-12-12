using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public static Workbench i;
    public List<Recipe> recipes = new List<Recipe>();

    private void Awake()
    {
        i = this;
    }

    public void Craft(Recipe recipe)
    {
        // suppose that is affordable from the UI
        Player.i.inventory.Remove(recipe.slot1);
        Player.i.inventory.Remove(recipe.slot2);

        Player.i.inventory.Add(recipe.result);
    }

}

[System.Serializable]
public class Recipe
{
    public ItemBase slot1, slot2, result;
    public int cost;

    public Recipe(ItemBase ingredient1, ItemBase ingredient2, ItemBase result)
    {
        slot1 = ingredient1;
        slot2 = ingredient2;
        this.result = result;
    }
}