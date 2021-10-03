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
    [SerializeField] Text itemDescription;

    [SerializeField] Text BalanceText;

    TraderController trader;
    List<ShopItemUI> shopUIs;

    int selected = 0;
    int category = 0;

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
                shopUIs[i].nameTxt.color = Color.cyan;
            }
            else
            {
                shopUIs[i].nameTxt.color = Color.black;
            }
        }

        if (shopUIs.Count == 0)
        {
            itemName.text = "";
            itemDescription.text = "there are no more items, for now.";
            itemPrice.text = "";
            return;
        }

        itemName.text = shopUIs[selected].item.Name;
        itemDescription.text = shopUIs[selected].item.description;
        itemPrice.text = shopUIs[selected].price.ToString();
    }

    public void HandleUpdate()
    {
        int prev = selected;
        
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        selected = Mathf.Clamp(selected, 0, shopUIs.Count - 1);

        if (prev != selected)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var player = FindObjectOfType<Player>();
            if(!sellMode)
            {
                if (player.kents < shopUIs[selected].price)
                    return;
                player.inventory.Add(shopUIs[selected].item);
                player.quest.goal.SomethingBought(trader, trader.inventory.GetSlots(category)[selected].item);
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
