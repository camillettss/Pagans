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
    List<ShopItemUI> shopUIs;

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

            shopUIs = new List<ShopItemUI>();
            foreach (var item in trader.inventory.GetSlots(category))
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
            foreach (var item in Player.i.inventory.GetSlots(category))
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

        if (shopUIs.Count == 0)
        {
            itemName.text = "";
            //itemDescription.text = "there are no more items, for now.";
            itemPrice.text = "";
            itemAmount.text = "";
            TotalPrice.text = "";
            itemIcon.enabled = false;
            return;
        }

        if(shopUIs.Count > 0)
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
        int prevcat = category;
        
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
            --category;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++category;

        category = Mathf.Clamp(category, 0, 1); // Hardcoded, fix!
        selected = Mathf.Clamp(selected, 0, shopUIs.Count - 1);

        if (prevcat != category)
            UpdateView();

        if (prev != selected)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var player = Player.i;
            if(!sellMode)
            {
                if (player.kents < shopUIs[selected].price)
                    return;
                player.inventory.Add(shopUIs[selected].item);

                if(Player.i.quest.goal != null)
                    player.quest.goal[0].SomethingBought(trader, trader.inventory.GetSlots(category)[selected].item);

                player.kents -= shopUIs[selected].price;

                trader.inventory.RemoveAt(category, selected);
            }
            else
            {
                player.kents += shopUIs[selected].price;
                player.inventory.Remove(shopUIs[selected].item);
            }

            selected = 0;
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
            switchToSell();
    }
}
