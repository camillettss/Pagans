using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public static Workbench i;
    public List<Recipe> recipes;

    private void Awake()
    {
        i = this;
    }


}

public class Recipe
{
    ItemBase slot1;
    ItemBase slot2;
    ItemBase result;
    public int cost;

    public List<ItemBase> GetData => new List<ItemBase>() { slot1, slot2, result };

    public Recipe(ItemBase ingredient1, ItemBase ingredient2, ItemBase result)
    {
        slot1 = ingredient1;
        slot2 = ingredient2;
        this.result = result;
    }
}