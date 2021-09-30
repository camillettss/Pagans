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

    TraderController trader;
    List<ShopItemUI> shopUIs;

    int selected = 0;
    int category = 0;

    private void Awake()
    {
        UpdateView();
    }

    public void SetTrader(TraderController trader)
    {
        this.trader = trader;
    }

    void UpdateView()
    {
        traderName.text = trader.Name;

        foreach (Transform child in scrollviewContent.transform)
            Destroy(child.gameObject);

        shopUIs = new List<ShopItemUI>();
        foreach (var item in trader.inventory.GetSlots(category))
        {
            var itemUI = Instantiate(itemPrefab, scrollviewContent.transform);
            itemUI.SetData(item, 5);

            shopUIs.Add(itemUI);
        }

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

        if(Input.GetKeyDown(KeyCode.Z))
        {
            FindObjectOfType<Player>().quest.goal.SomethingBought(trader, trader.inventory.GetSlots(category)[selected].item);
        }
    }
}
