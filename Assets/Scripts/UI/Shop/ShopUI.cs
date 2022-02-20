using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour, UIController
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
        selected = 0;
        UpdateView();
        Player.i.playerInput.SwitchCurrentActionMap("UI");
    }

    public void SetTrader(TraderController trader)
    {
        sellMode = false;
        this.trader = trader;
    }

    public void switchToSell()
    {
        sellMode = !sellMode;
        selected = 0;
        amount = 1;
        UpdateView();
    }

    public void switchToSell(bool sellmode)
    {
        sellMode = sellmode;
        selected = 0;
        UpdateView();
    }

    void UpdateView()
    {
        foreach (Transform child in scrollviewContent.transform)
            Destroy(child.gameObject);

        shopUIs = new List<ShopItemUI>();

        if (!sellMode)
        {
            traderName.text = trader.Name;

            if (trader.inventory.GetShopSlots().Count != 0)
            {
                foreach (var item in trader.inventory.GetShopSlots())
                {
                    var itemUI = Instantiate(itemPrefab, scrollviewContent.transform);
                    itemUI.SetData(item, item.item.price);

                    shopUIs.Add(itemUI);
                }
            }
        }
        else
        {
            traderName.text = "you";

            if(Player.i.inventory.GetShopSlots().Count != 0)
            {
                foreach (var item in Player.i.inventory.GetShopSlots())
                {
                    var itemUI = Instantiate(itemPrefab, scrollviewContent.transform);
                    itemUI.SetData(item, item.item.price);

                    shopUIs.Add(itemUI);
                }
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

    IEnumerator unaffordableTextAnimation()
    {
        var stoppedSelected = selected;

        shopUIs[stoppedSelected].nameTxt.color = Color.red;
        yield return new WaitForSeconds(.3f);
        shopUIs[stoppedSelected].nameTxt.color = Color.black;
        /*int i = 1;
        while (i < 3)
        {
            shopUIs[stoppedSelected].nameTxt.color = Color.red;
            yield return new WaitForSeconds(.15f);
            shopUIs[stoppedSelected].nameTxt.color = Color.black;
            yield return new WaitForSeconds(.1f);
            i++;
        }*/
    }

    public void HandleUpdate()
    {
        int prev = selected;
        int pamount = amount;
        
        /*if(Input.GetKeyDown(KeyCode.X))
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
        /*if (sellMode && shopUIs.Count > 0 && pamount != amount)
            amount = Mathf.Clamp(amount, 1, Player.i.inventory.findItem(shopUIs[selected].item).count);

        if(pamount != amount)
        {
            try
            {
                if (sellMode) // lo trovi nell'inv del player
                {
                    amount = Mathf.Clamp(amount, 1, Player.i.inventory.findItem(shopUIs[selected].item).count);
                }
                else
                {
                    amount = Mathf.Clamp(amount, 1, trader.inventory.findItem(shopUIs[selected].item).count);
                }
            }
            catch (System.NullReferenceException)
            {
                print("oggetto rimosso già dall'inventario ma sono stati lasciati dei riferimenti attivi nella UI.");
            }
        }
        /*else if (!sellMode && shopUIs.Count > 0)
            amount = Mathf.Clamp(amount, 1, trader.inventory.findItem(shopUIs[selected].item).count); //FIX
        

        selected = Mathf.Clamp(selected, 0, shopUIs.Count - 1);

        if (prev != selected || pamount != amount)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var player = Player.i;
            if(!sellMode)
            {
                if (player.kents < shopUIs[selected].price*amount)
                {
                    StartCoroutine(unaffordableTextAnimation());
                    return;
                }
                StoryEventHandler.i.AddToInventory(shopUIs[selected].item, amount);

                if(Player.i.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingBought(trader, shopUIs[selected].item, amount);

                player.kents -= shopUIs[selected].price * amount;

                for (int i = 0; i < amount; i++)
                    trader.inventory.Remove(shopUIs[selected].item);
            }
            else
            {
                player.kents += shopUIs[selected].price * amount;

                if (player.quest != null && player.quest.goal != null && player.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingSelled(trader, shopUIs[selected].item, amount);

                for (int i=0; i<amount; i++)
                {
                    trader.inventory.Add(shopUIs[selected].item);
                    player.inventory.Remove(shopUIs[selected].item);
                }
            }

            selected = 0;
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
            switchToSell();*/
    }

    private void OnDisable()
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
        Player.i.playerInput.actions["Navigate"].performed -= onNavigate;
        Player.i.playerInput.actions["Cancel"].performed -= onCancel;
    }

    private void OnEnable()
    {
        //Player.i.playerInput.SwitchCurrentActionMap("UI");

        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            var player = Player.i;
            if (!sellMode)
            {
                if (player.kents < shopUIs[selected].price * amount)
                {
                    StartCoroutine(unaffordableTextAnimation());
                    return;
                }
                StoryEventHandler.i.AddToInventory(shopUIs[selected].item, amount);

                if (Player.i.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingBought(trader, shopUIs[selected].item, amount);

                player.kents -= shopUIs[selected].price * amount;

                for (int i = 0; i < amount; i++)
                    trader.inventory.Remove(shopUIs[selected].item);
            }
            else
            {
                player.kents += shopUIs[selected].price * amount;

                if (player.quest != null && player.quest.goal != null && player.quest.goal.Count > 0)
                    player.quest.goal[0].SomethingSelled(trader, shopUIs[selected].item, amount);

                for (int i = 0; i < amount; i++)
                {
                    trader.inventory.Add(shopUIs[selected].item);
                    player.inventory.Remove(shopUIs[selected].item);
                }
            }

            selected = 0;
            UpdateView();
        }
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        GameController.Instance.state = GameState.FreeRoam;
        gameObject.SetActive(false);
        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        if(shopUIs.Count > 0)
        {
            var input = ctx.ReadValue<Vector2>();
            var pamount = amount;

            if (input.y < 0) ++selected;
            else if (input.y > 0) --selected;

            if (input.x < 0) --amount;
            else if (input.x > 0) ++amount;

            //category = Mathf.Clamp(category, 0, 1); // Hardcoded, fix!
            if (sellMode && shopUIs.Count > 0 && pamount != amount)
                amount = Mathf.Clamp(amount, 1, Player.i.inventory.findItem(shopUIs[selected].item).count);

            selected = Mathf.Clamp(selected, 0, shopUIs.Count-1);

            if (pamount != amount)
            {
                try
                {
                    if (sellMode) // lo trovi nell'inv del player
                    {
                        amount = Mathf.Clamp(amount, 1, Player.i.inventory.findItem(shopUIs[selected].item).count);
                    }
                    else
                    {
                        amount = Mathf.Clamp(amount, 1, trader.inventory.findItem(shopUIs[selected].item).count);
                    }
                }
                catch (System.NullReferenceException)
                {
                    print("oggetto rimosso già dall'inventario ma sono stati lasciati dei riferimenti attivi nella UI.");
                }
            }
            else if (!sellMode && shopUIs.Count > 0)
                amount = Mathf.Clamp(amount, 1, trader.inventory.findItem(shopUIs[selected].item).count); //FIX


            selected = Mathf.Clamp(selected, 0, shopUIs.Count - 1);

            UpdateSelection();
        }
    }
}
