using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantingUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] EnchSlotUI slotPrefab;
    [SerializeField] LineAnimator lineAnimator;

    [SerializeField] Image DustIcon;
    [SerializeField] Text RuneSlot1;
    [SerializeField] Text RuneSlot2;
    [SerializeField] Image GemIcon;
    [SerializeField] Image itemIcon;

    [SerializeField] GameObject EntireScrollview;
    [SerializeField] GameObject CircleSlots;

    ItemBase item = null;
    List<EnchSlotUI> slotUIs;
    int selected = 0;
    int slotSelected = 0; // 0 = rune slot 1, 1 = "2, 2 = dust slot, 3 = gem slot

    ItemConfig config;

    private void Awake()
    {
        print("EnchUI awaked");
        UpdateContents();
        UpdateCirclesSelection();
        UpdateRunes();
        slotSelected = 0;
        EntireScrollview.SetActive(false);
    }

    public void Open(ItemBase item)
    {
        config = new ItemConfig();
        config.Setup(item);
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
            foreach (var dust in GameController.Instance.player.inventory.dusts)
            {
                var dustSlot = Instantiate(slotPrefab, content.transform);
                dustSlot.SetData(dust);

                slotUIs.Add(dustSlot);
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

        if(config.dust != null)
        {
            DustIcon.enabled = true;
            DustIcon.sprite = config.dust.icon;
        }

        if (config.runes[0] != null)
            RuneSlot1.text = config.runes[0].Letter;
        else
            RuneSlot1.text = "";

        if (config.runes[1] != null)
            RuneSlot2.text = config.runes[1].Letter;
        else
            RuneSlot2.text = "";

        // Add gems
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].nameTxt.color = Color.cyan;
            else
                slotUIs[i].nameTxt.color = Color.black;
        }
    }

    void UpdateCirclesSelection()
    {
        for(int i=0; i<CircleSlots.transform.childCount; i++) 
        {
            if (i == slotSelected)
                CircleSlots.transform.GetChild(i).GetComponent<Image>().color = Color.cyan;
            else
                CircleSlots.transform.GetChild(i).GetComponent<Image>().color = Color.black;
        }
    }
    public void HandleUpdate()
    {

        if (EntireScrollview.activeSelf)
        {
            // selecting an enchantment
            int prev = selected;
            if (Input.GetKeyDown(KeyCode.X))
            {
                EntireScrollview.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selected;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selected;

            selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

            if (prev != selected)
                UpdateSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if(slotSelected == 0 || slotSelected == 1) // Rune
                {
                    config.runes[slotSelected] = slotUIs[selected].runeItem;
                    //FindObjectOfType<Player>().inventory.runes.Remove(slotUIs[selected].runeItem);
                }
                else if(slotSelected == 2) // Dust
                {
                    config.dust = slotUIs[selected].dustItem;
                    //item.dust = slotUIs[selected].dustItem;
                }

                UpdateContents();
                UpdateRunes();
            }

            // PERCHE CAZZO ITEM.RUNES.SLOTS VIENE SOVRASCRITTO SE NON HA RIFERIMENTI???
            // succede solo con le rune ._.

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
                ++slotSelected;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --slotSelected;

            slotSelected = Mathf.Clamp(slotSelected, 0, CircleSlots.transform.childCount - 1); // 3 is hardcoded, i will fix this.  // four hours later: im a stupid bitch. // this is no more hardcoded but i love these comments so ill leave them.

            if (prevSlot != slotSelected)
                UpdateCirclesSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Enchant
                EntireScrollview.SetActive(true);
                UpdateContents();
            }
        }
    }

}

public class ItemConfig
{
    public List<Rune> runes;

    public Dust dust;

    public void Setup(ItemBase item)
    {
        Debug.Log($"item rune.0: {item.runes.slots[0]}");
        runes = item.runes.slots;
        dust = item.dust;
    }
}