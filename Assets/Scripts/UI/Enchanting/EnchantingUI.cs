using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantingUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] EnchSlotUI slotPrefab;

    [SerializeField] Image DustIcon;
    [SerializeField] Text RuneSlot1;
    [SerializeField] Text RuneSlot2;
    [SerializeField] Image GemIcon;
    [SerializeField] Image itemIcon;
    [SerializeField] Image accceptButton;

    [SerializeField] Sprite selectedCircle;
    [SerializeField] Sprite deselectedCircle;

    [SerializeField] GameObject EntireScrollview;
    [SerializeField] GameObject CircleSlots;

    ItemBase item = null;
    List<EnchSlotUI> slotUIs;
    int selected = 0;
    int slotSelected = 0; // 0 = rune slot 1, 1 = "2, 2 = dust slot, 3 = gem slot

    AnchestralConfiguration config;

    private void Awake()
    {
        print("EnchUI awakening");
        config = new AnchestralConfiguration(item);
        //config.setup(item); // finisce con successo, esce senza modifiche 0_0
        print($"config original rune0 before calling UpdateContents: {config.rune0}"); // is Rune
        UpdateContents(); // becomes null :/ idk y
        print($"config original rune0 after calling UpdateContents: {config.rune0}, now updating circles."); // is Rune
        UpdateCirclesSelection();
        print($"config original rune0 after calling UpdateCirclesSelection and before runes update: {config.rune0}"); // is Rune
        UpdateRunes();
        print($"config original rune0 after calling UpdateRunes: {config.rune0}"); // is Rune
        slotSelected = 0;
        EntireScrollview.SetActive(false);
    }

    public void Open(ItemBase item)
    {
        print($"setted up with rune0: {item.runes.slots[0]}");
        GameController.Instance.state = GameState.Enchanting;
        this.item = item;
        gameObject.SetActive(true);
        Awake();
    }

    void UpdateContents()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<EnchSlotUI>();

        // costruisci in base allo slot

        if(slotSelected == 0 || slotSelected == 1) // rune slot 1 or 2 
        {
            foreach (var rune in GameController.Instance.player.inventory.runes)
            {
                var runeSlot = Instantiate(slotPrefab, content.transform);
                runeSlot.SetData(rune);

                slotUIs.Add(runeSlot);
            }
        }
        else if(slotSelected == 2) // dust
        {
            foreach (var dust in Player.i.inventory.dusts)
            {
                var dustSlot = Instantiate(slotPrefab, content.transform);
                dustSlot.SetData(dust);

                slotUIs.Add(dustSlot);
            }
        }
        else if(slotSelected == 3) // gem
        {
            foreach(var gem in Player.i.inventory.gems)
            {
                var gemSlot = Instantiate(slotPrefab, content.transform);
                gemSlot.SetData(gem);

                slotUIs.Add(gemSlot);
            }
        }

        itemIcon.sprite = item.icon;
        UpdateRunes();
        UpdateSelection();
    }

    void ResetRunes()
    {
        GemIcon.enabled = false;
        DustIcon.enabled = false;
        RuneSlot1.text = "";
        RuneSlot2.text = "";
    }

    void UpdateRunes()
    {
        ResetRunes();

        print($"updating runes, rune0 is:{config.rune0}");

        if(config.dust != null)
        {
            DustIcon.enabled = true;
            DustIcon.sprite = config.dust.icon;
        }

        if (config.rune0 != null)
            RuneSlot1.text = config.rune0.Letter;
        else
            RuneSlot1.text = "";

        if (config.rune1 != null)
            RuneSlot2.text = config.rune1.Letter;
        else
            RuneSlot2.text = "";

        if (config.hasGem())
        {
            GemIcon.enabled = true;
            GemIcon.sprite = config.gemSprite();
            accceptButton.gameObject.SetActive(true);
        }
        else
        {
            GemIcon.enabled = false;
            accceptButton.gameObject.SetActive(false);
        }
        
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].nameTxt.color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].nameTxt.color = GameController.Instance.unselectedDefaultColor;
        }
    }

    void UpdateCirclesSelection()
    {
        for(int i=0; i<CircleSlots.transform.childCount; i++) 
        {
            if (i == slotSelected)
                CircleSlots.transform.GetChild(i).GetComponent<Image>().sprite = selectedCircle;
            else
                CircleSlots.transform.GetChild(i).GetComponent<Image>().sprite = deselectedCircle;
        }
    }
    public void HandleUpdate()
    {
        /*

        if (EntireScrollview.activeSelf)
        {
            // selecting an enchantment
            int prev = selected;
            if (Input.GetKeyDown(KeyCode.X))
            {
                EntireScrollview.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
                --selected;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                ++selected;

            selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

            if (prev != selected)
                UpdateSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if(slotSelected == 0 || slotSelected == 1) // Rune
                {
                    print($"applying a rune:{slotUIs[selected].runeItem}, name:{slotUIs[selected].runeItem.Name}, on {config}");
                    config.SetRune(slotSelected, slotUIs[selected].runeItem);
                    //FindObjectOfType<Player>().inventory.runes.Remove(slotUIs[selected].runeItem);
                    UpdateRunes();
                }
                else if(slotSelected == 2) // Dust
                {
                    config.SetDust(slotUIs[selected].dustItem);
                }
                else if(slotSelected == 3) // gems
                {
                    config.SetGem(slotUIs[selected].gemItem);
                }

                UpdateContents();
                //UpdateRunes();
            }

        }
        else
        {
            // selecting a slot.
            int prevSlot = slotSelected;
            if (Input.GetKeyDown(KeyCode.X))
            {
                gameObject.SetActive(false);
                //GameController.Instance.equipmentUI.gameObject.SetActive(true);
                GameController.Instance.state = GameState.FreeRoam;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
                slotSelected = 3;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                slotSelected = 2;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                slotSelected = 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                slotSelected = 0;

            if (prevSlot != slotSelected)
                UpdateCirclesSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Enchant
                EntireScrollview.SetActive(true);
                UpdateContents();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if(config.hasGem())
                {
                    print("buying");
                    Player.i.inventory.gems.Remove(config.Gem);
                    config.Upload(item);
                }
            }
        }*/
    }

}

public class AnchestralConfiguration
{
    Rune _rune0;
    Rune _rune1;

    Dust _dust;

    ItemBase gem;

    public Dust dust => _dust;
    public Rune rune0 => _rune0;
    public Rune rune1 => _rune1;
    public ItemBase Gem => gem;

    public void setup(ItemBase item)
    {
        Debug.Log($"setting up, r0:{item.runes.slots[0]} goes in _r0:{_rune0}");
        _rune0 = item.runes.slots[0];
        _rune0 = item.runes.slots[1];
        _dust = item.dust;
        Debug.Log($"setted up, rune0 is {_rune0}"); 
    }

    public AnchestralConfiguration(ItemBase item)
    {
        Debug.Log("resetting config");
        _rune0 = null;
        _rune1 = null;
        _dust = null;
        gem = null;
        setup(item);
    }

    public void SetRune(int index, Rune rune)
    {
        if (index == 0)
            _rune0 = rune;

        else if (index == 1)
            _rune1 = rune;
    }

    public void SetDust(Dust newdust)
    {
        _dust = newdust;
    }

    public void SetGem(ItemBase gem)
    {
        this.gem = gem;
    }

    public Sprite gemSprite()
    {
        if (hasGem())
            return gem.icon;
        else
            return null;
    }

    public bool hasGem()
    {
        if (gem == null)
            return false;
        else
            return true;
    }

    public void Upload(ItemBase item)
    {
        item.runes.slots[0] = rune0;
        item.runes.slots[1] = rune1;
        item.dust = dust;
    }

}