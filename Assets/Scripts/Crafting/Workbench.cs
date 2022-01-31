using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public static Workbench i;
    public List<Recipe> TableRecipes = new List<Recipe>();
    public List<Recipe> NavalRecipes = new List<Recipe>();

    private void Awake()
    {
        i = this;
    }

    public List<Recipe> GetRecipes(CraftingType type)
    {
        if (type == CraftingType.Table)
            return TableRecipes;
        else if (type == CraftingType.Naval)
            return NavalRecipes;

        return new List<Recipe>();
    }

    public void Craft(Recipe recipe)
    {
        // suppose that is affordable from the UI
        Player.i.inventory.Remove(recipe.slot1);
        Player.i.inventory.Remove(recipe.slot2);

        Player.i.inventory.Add(recipe.result);
    }

    public void Spawn(Boat prefab, Transform boatSpawner) // trova un modo di adattare e mettere Boat
    {
        var boat = Instantiate(prefab);
        boat.transform.position = boatSpawner.position;
    }

}

[System.Serializable]
public class Recipe
{
    public ItemBase slot1, slot2, result;
    public Boat boat_result;

    public Recipe(ItemBase ingredient1, ItemBase ingredient2, ItemBase result)
    {
        slot1 = ingredient1;
        slot2 = ingredient2;
        this.result = result;
    }
}