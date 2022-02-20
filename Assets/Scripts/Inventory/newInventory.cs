using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class newInventory : MonoBehaviour
{
    // bookmarks
    [SerializeField] Sprite unselectedBookmark;
    [SerializeField] Sprite selectedBookmark;
    // category count text
    [SerializeField] Text category_count;
    // pagina sinistra - choosing
    [SerializeField] GameObject tags_container;
    [SerializeField] Text categoryText;
    [SerializeField] GameObject content;
    [SerializeField] SlotUI itemUIprefab;
    // pagina destra - details
    [SerializeField] Text selectedName;
    [SerializeField] Text selectedDescription;
    [SerializeField] Image selectedIcon;
    [SerializeField] Text actualLPS;

    // tips
    [SerializeField] Text leftTip;
    [SerializeField] Text rightTip;

    [SerializeField] Image stats;
    [SerializeField] Text innerStatsText;

    int bookmark = 0;
    int category = 0;
    int selected = 0;

    List<SlotUI> slotUIs;

    public const int weapons_category = 0;
    public const int tools_category = 1;
    public const int shields_category = 2;
    public const int rings_category = 3;

    public const int runes_category = 0;
    public const int dusts_category = 1;
    public const int gems_category = 2;

    private void OnEnable()
    {
        UpdateView();
        Player.i.playerInput.SwitchCurrentActionMap("UI");
        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
        Player.i.playerInput.actions["ExtraNavigation"].performed += xtraNav;
    }
     
    private void OnDisable() // DONT FORGET THIS MF
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
        Player.i.playerInput.actions["Navigate"].performed -= onNavigate;
        Player.i.playerInput.actions["Cancel"].performed -= onCancel;
        Player.i.playerInput.actions["ExtraNavigation"].performed -= xtraNav;
        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    public void UpdateView(bool resetCategory=true)
    {
        slotUIs = new List<SlotUI>();

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        selected = 0;
        if(resetCategory)
            category = 0;

        foreach (var item in Player.i.inventory.GetByBookmark(bookmark)[category])
        {
            var slotUI = Instantiate(itemUIprefab, content.transform);
            slotUI.SetData(item);

            slotUIs.Add(slotUI);
        }

        if (bookmark == 3 && category == 1)
        {
            actualLPS.gameObject.SetActive(true);
            actualLPS.text = $"EXP: {Player.i.experience}";
        }
        else
            actualLPS.gameObject.SetActive(false);

        if(bookmark == 0)
        {
            leftTip.text = "(Z) Equipaggia"; // make this dynamic
            rightTip.text = "";
        }
        else if(bookmark == 1)
        {
            leftTip.text = "(Z) Usa";
            rightTip.text = "";
        }
        else if(bookmark == 2)
        {
            leftTip.text = "(Z) Equipaggia";
            rightTip.text = "(S) Sacrifica";
        }
        else if(bookmark == 3)
        {
            leftTip.text = "(Z) Usa";
            rightTip.text = "";
        }

        if(slotUIs.Count == 0)
        {
            leftTip.text = "";
            rightTip.text = "";
        }

        categoryText.text = Inventory.GetCategoryName(bookmark, category);
        UpdateSelection();
    }

    void UpdateSelection()
    {

        for(int i=0; i<tags_container.transform.childCount; i++)
        {
            if (i == bookmark)
                tags_container.transform.GetChild(i).GetComponent<Image>().sprite = selectedBookmark;
            else
                tags_container.transform.GetChild(i).GetComponent<Image>().sprite = unselectedBookmark;
        }
        category_count.text = $"{category+1}/{Inventory.BookmarkSize(bookmark)}";

        if (slotUIs.Count < 1) // turn all off and write an alert.
        {
            selectedIcon.enabled = false;
            selectedName.text = "";
            selectedDescription.text = "no items here for now, i hope.";
            return;
        }
        else
            selectedIcon.enabled = true;

        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected)
            {
                if(bookmark == 0)
                {
                    if(Player.i.inventory.equips[category] == selected)
                        slotUIs[i].nameTxt.color = GameController.Instance.equipedSelectedColor;

                    else if(Player.i.inventory.secondaryWeapon == selected)
                        slotUIs[i].nameTxt.color = GameController.Instance.secondarySelectedColor;

                    else
                        slotUIs[i].nameTxt.color = GameController.Instance.selectedDefaultColor;
                }
                else
                    slotUIs[i].nameTxt.color = GameController.Instance.selectedDefaultColor;
            }

            else
            {
                if (bookmark == 0)
                {
                    if (Player.i.inventory.equips[category] == i)
                        slotUIs[i].nameTxt.color = GameController.Instance.equipedDefaultColor;

                    else if (Player.i.inventory.secondaryWeapon == i)
                        slotUIs[i].nameTxt.color = GameController.Instance.secondaryDefaultColor;

                    else
                        slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
                }
                else
                    slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }

        if (bookmark == 0)
        {
            stats.gameObject.SetActive(true);
            innerStatsText.text = $"Damage: {slotUIs[selected].item.closeDamage}";
        }
        else
            stats.gameObject.SetActive(false);

        selectedName.text = slotUIs[selected].item.Name;
        selectedDescription.text = slotUIs[selected].item.description;
        selectedIcon.sprite = slotUIs[selected].item.icon;
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        if (slotUIs.Count <= 0) // dont really now how it can be negative, but who knows
            return;

        if (bookmark == 0) // armi, scudi, strumenti e anelli
        {
            Player.i.inventory.Equip(category, selected);
            GameController.Instance.EvH.OnEquip(slotUIs[selected].item);
            GameController.Instance.hotbar.UpdateItems();
            UpdateView(false);
        }
        else if (bookmark == 3 && category == 1) // libri
        {
            slotUIs[selected].item.Use(Player.i); // usa il libro
            learn_book();
        }
        else if (bookmark == 1 && category == 0) // runes
        {
            slotUIs[selected].item.Use(Player.i);
        }
        else if (bookmark == 2 && category == 0) // consumabili
        {
            if (slotUIs[selected].item != null && slotUIs[selected].item == Player.i.inventory.extraSlot.item)
            {
                if (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null && Player.i.inventory.extraSlot.item.GetType() == typeof(Seeds))
                    ((Seeds)Player.i.inventory.extraSlot.item).onUnequip();
                Player.i.inventory.extraSlot = null;
                return;
            }

            if (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null && Player.i.inventory.extraSlot.item.GetType() == typeof(Seeds))
                ((Seeds)Player.i.inventory.extraSlot.item).onUnequip();

            var invslot = Player.i.inventory.findItem(slotUIs[selected].item);

            if (invslot.item.GetType() == typeof(Seeds))
            {
                ((Seeds)invslot.item).onEquip();
            }

            Player.i.inventory.extraSlot = invslot;
            GameController.Instance.extraItemUI.HandleUpdate();
        }
    }

    public void onCancel(InputAction.CallbackContext _)
    {
        gameObject.SetActive(false);
        GameController.Instance.state = GameState.FreeRoam; // hardcode kinda sus
        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        int prev_sel = selected;
        int prev_cat = category;

        var input = ctx.ReadValue<Vector2>();

        print($"input: {input}");

        // check for change selected
        if (input.y < 0) selected++;
        else if (input.y > 0) selected--;

        // check for change category
        if(input.x < 0) category--;
        else if (input.x > 0) category++;

        category = Mathf.Clamp(category, 0, Inventory.BookmarkSize(bookmark) - 1);
        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if (prev_cat != category)
            UpdateView(false);

        if (selected != prev_sel)
            UpdateSelection();
}

    void xtraNav(InputAction.CallbackContext ctx) // changing bookmark
    {
        var input = ctx.ReadValue<Vector2>();

        // check for change selected
        if (input.y < 0) bookmark++;
        else if (input.y > 0) bookmark--;

        bookmark = Mathf.Clamp(bookmark, 0, tags_container.transform.childCount - 1); // 0 - 3, teoricamente.

        UpdateView();
    }

    public void HandleUpdate()
{
    if(GameController.Instance.sacrificeUI.gameObject.activeSelf)
    {
        GameController.Instance.sacrificeUI.HandleUpdate();
    }

    else
    {
        int prev_sel = selected;
        int prev_cat = category;
        int prev_bok = bookmark;

        /*if (Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
            ExtraItemUI.i.HandleUpdate();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            --category;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++category;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            ++bookmark;
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            --bookmark;

        bookmark = Mathf.Clamp(bookmark, 0, tags_container.transform.childCount - 1); // 0, 3 tecnicamente.
        category = Mathf.Clamp(category, 0, Inventory.BookmarkSize(bookmark) - 1);
        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if (prev_bok != bookmark)
            UpdateView();

        if (prev_cat != category)
            UpdateView(false);

        if (selected != prev_sel)
            UpdateSelection();

        /*if (Input.GetKeyDown(KeyCode.S))
        {
            if(bookmark==2 && category == 0) // puoi sacrificare solo i consumabili
                GameController.Instance.sacrificeUI.Open(slotUIs[selected].item);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            // lavora un oggetto
            GameController.Instance.OpenState(GameState.CraftUI, craftItem:slotUIs[selected].item);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (slotUIs.Count <= 0)
                return;

            if (bookmark == 0) // armi, scudi, strumenti e anelli
            {
                Player.i.inventory.Equip(category, selected);
                GameController.Instance.EvH.OnEquip(slotUIs[selected].item);
                GameController.Instance.hotbar.UpdateItems();
                UpdateView(false);
            }
            else if (bookmark == 3 && category == 1) // libri
            {
                slotUIs[selected].item.Use(Player.i); // usa il libro
                learn_book();
            }
            else if (bookmark == 1 && category == 0) // runes
            {
                slotUIs[selected].item.Use(Player.i);
            }
            else if (bookmark == 2 && category == 0) // consumabili
            {
                if (slotUIs[selected].item != null && slotUIs[selected].item == Player.i.inventory.extraSlot.item)
                {
                    if (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null && Player.i.inventory.extraSlot.item.GetType() == typeof(Seeds))
                        ((Seeds)Player.i.inventory.extraSlot.item).onUnequip();
                    Player.i.inventory.extraSlot = null;
                    return;
                }

                if (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null && Player.i.inventory.extraSlot.item.GetType() == typeof(Seeds))
                    ((Seeds)Player.i.inventory.extraSlot.item).onUnequip();

                var invslot = Player.i.inventory.findItem(slotUIs[selected].item);

                if(invslot.item.GetType() == typeof(Seeds))
                {
                    ((Seeds)invslot.item).onEquip();
                }

                Player.i.inventory.extraSlot = invslot;
            }
        }*/
    }
}

    void learn_book()
    {
        slotUIs[selected].GetComponent<Animator>().SetTrigger("die");
        //yield return new WaitForSeconds(.5f);
        UpdateView(false);
    }
}
