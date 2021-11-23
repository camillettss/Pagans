using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject scrollviewContent;
    [SerializeField] ShopItemUI itemPrefab;

    [SerializeField] Text traderName;

    [SerializeField] Text itemName;
    [SerializeField] Text itemPrice;
    [SerializeField] Text itemAmount;
    [SerializeField] Text TotalPrice;
    [SerializeField] Image itemIcon;

    [SerializeField] Text BalanceText;

    TraderController trader;
    List<ShopItemUI> shopUIs = new List<ShopItemUI>();

    int selected = 0;
    int category = 0;
    int amount = 1;

    bool sellMode = false;

    private void Awake()
    {
        sellMode = false;
        UpdateView();
    }

    public void SetTrader(TraderController trader)
    {
        sellMode = false;
        this.trader = trader;
    }

    void switchToSell()
    {
        sellMode = !sellMode;
        UpdateView();
    }

    void UpdateView()
    {
        if(!sellMode)
        {
            traderName.text = trader.Name;

            foreach (Transform child in scrollviewContent.transform)
                Destroy(child.gameObject);

            foreach (var item in trader.inventory.GetShopSlots())
            {
                var itemUI = Instantiate(itemPrefab, scrollviewContent.transform);
                itemUI.SetData(item, item.item.price);

                shopUIs.Add(itemUI);
            }
        }
        else
        {
            traderName.text = "you";

            foreach (Transform child in scrollviewContent.transform)
                Destroy(child.gameObject);

            shopUIs = new List<ShopItemUI>();
            foreach (var item in Player.i.inventory.GetShopSlots())
            {
                var itemUI = Instantiate(itemPrefab, scrollviewContent.transform);
                itemUI.SetData(item, item.item.price);

                shopUIs.Add(itemUI);
            }
        }

        BalanceText.text = $"kents: {Player.i.kents}";
        UpdateSelection();
    }

    public void UpdateSelection()
    {
        for(int i=0; i<shopUIs.Count; i++)
        {
            if(i==selected)
            {
                shopUIs[i].nameTxt.color = GameController.Instance.selectedDefaultColor;
            }
            else
            {
                shopUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }

        if (shopUIs.Count < 1)
        {
            itemName.text = "";
            //itemDescription.text = "there are no more items, for now.";
            itemPrice.text = "";
            itemAmount.text = "";
            TotalPrice.text = "";
            itemIcon.enabled = false;
            return;
        }
        else
        {
            itemName.text = shopUIs[selected].item.Name;
            itemPrice.text = $"{shopUIs[selected].item.price}";
            itemAmount.text = $"{amount}";
            itemPrice.text = shopUIs[selected].price.ToString();
            TotalPrice.text = $"{shopUIs[selected].item.price*amount}";
            itemIcon.enabled = true;
            itemIcon.sprite = shopUIs[selected].item.icon;
        }
    }

    public void HandleUpdate()
    {
        int prev = selected;
        int pamount = amount;
        
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            --amount;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++amount;

        //category = Mathf.Clamp(category, 0, 1); // Hardcoded, fix!
        if (sellMode && shopUIs.Count > 0)
            amount = Mathf.Clamp(amount, 1, Player.i.inventory.findItem(shopUIs[selected].item).count);
        /*else if (!sellMode && shopUIs.Count > 0)
            amount = Mathf.Clamp(amount, 1, trader.inventory.findItem(shopUIs[selected].item).count); //FIX
        */

        selected = Mathf.Clamp(selected, 0, shopUIs.Count - 1);

        if (prev != selected || pamount != amount)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var player = Player.i;
            if(!sellMode)
            {
                if (player.kents*amount < shopUIs[selected].price)
                    return;
                StoryEventHandler.i.AddToInventory(shopUIs[selected].item);

                if(Player.i.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingBought(trader, shopUIs[selected].item);

                player.kents -= shopUIs[selected].price * amount;

                for (int i = 0; i < amount; i++)
                    trader.inventory.Remove(shopUIs[selected].item);
            }
            else
            {
                player.kents += shopUIs[selected].price * amount;

                for(int i=0; i<amount; i++)
                    player.inventory.Remove(shopUIs[selected].item);

                if (player.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingSelled(trader, shopUIs[selected].item);
            }

            selected = 0;
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
            switchToSell();
    }
}
