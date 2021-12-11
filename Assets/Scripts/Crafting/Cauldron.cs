using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour, IEntity
{
    public bool isCooking = false;
    public ItemBase ingredient;
    public ItemBase result;

    public int count;

    public void Interact(Player player)
    {
        GameController.Instance.OpenState(GameState.Cauldron, c:this);
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
    }

    public IEnumerator Cook(ItemBase item)
    {
        isCooking = true;
        ingredient = item;
        result = item.cookedResult;

        yield return new WaitForSeconds(item.cookTime);

        isCooking = false;

        GameController.Instance.cauldronUI.UpdateContents();

        print("finished cooking.");
    }
}
