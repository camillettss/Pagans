using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    [SerializeField] Image startItemIcon;
    [SerializeField] Image resultIcon;
    [SerializeField] Image methodIcon;

    [SerializeField] Text itemNameText;
    [SerializeField] Text workDescription;
    [SerializeField] Text experienceReward;
    [SerializeField] Text doText;

    bool isAffordable = false;
    ItemBase item;

    public void HandleUpdate()
    {
        // TODO: add worktype change
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.Inventory;
            try
            {
                FindObjectOfType<newInventory>().UpdateView();
            }
            catch
            {
                print("this point should never be reached. <newInventory> object must always be found,");
            }
            gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // Do
            if(isAffordable)
            {
                if(item.hasItemCost)
                    Player.i.inventory.Remove(item.craftCost, item.craftCostCount);

                Player.i.experience += item.craftExperienceReward;
                Player.i.inventory.Add(item.handcraftDerivatedItem, item.craftResultQuantity);
                Player.i.inventory.Remove(item);
            }
        }*/
    }

    public void UpdateContents(ItemBase item)
    {
        this.item = item;

        checkPrice();

        startItemIcon.sprite = item.icon;
        if (item.hasItemCost)
        {
            resultIcon.sprite = item.handcraftDerivatedItem.icon;
            workDescription.text = $"questo lavoro ti darà 1 {item.handcraftDerivatedItem.Name}, ma ti costerà {item.craftCostCount} {item.craftCost.Name}. otterrai anche {item.craftExperienceReward} punti esperienza";
            // TODO: add levels
        }
        workDescription.text = $"questo lavoro ti darà 1 {item.handcraftDerivatedItem.Name}, e non ti costerà nulla. otterrai anche {item.craftExperienceReward} punti esperienza";
        methodIcon.sprite = item.craftMethodIcon;
        itemNameText.text = item.Name;
        experienceReward.text = $"{item.craftExperienceReward} XP";

        if (isAffordable)
        {
            doText.color = GameController.Instance.AffordableGreenColor;
        }
        else
            doText.color = GameController.Instance.UnaffordableRedColor;
    }

    void checkPrice()
    {
        if (!item.hasItemCost)
        {
            isAffordable = true;
            return;
        }
        else if(Player.i.inventory.alreadyInStock(item.craftCost))
        {
            if(Player.i.inventory.findItem(item.craftCost).count >= item.craftCostCount)
            {
                isAffordable = true;
                return;
            }
        }
        isAffordable = false;
    }
}
