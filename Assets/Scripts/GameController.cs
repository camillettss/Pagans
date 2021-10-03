using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Rendering;

public enum GameState { FreeRoam, Menu, Dialogue, Quest, Inventory, Equipment, Shop, Quests, Enchanting, Chest };
public class GameController : MonoBehaviour
{
    public GameObject EssentialObjectsPrefab;

    public Player player;
    [SerializeField] MenuController menu;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] public EquipmentUI equipmentUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] QuestsUI questsUI;
    [SerializeField] EnchantingUI enchantingUI;
    [SerializeField] public Hotbar hotbar;
    [SerializeField] ChestUI basicChestUI;
    [SerializeField] Volume ppv; // post processing volume

    float tick=60, seconds, mins, hours, days = 1;
    bool activateLights;

    public DialogueManager dialogueBox;
    public QuestController questWindow;

    [HideInInspector] public GameState state = GameState.FreeRoam;
    [HideInInspector] public NPCController ActiveNPC;

    public static GameController Instance;
    public EnchantingUI EnchUI => enchantingUI;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        hotbar.UpdateItems();
        hotbar.gameObject.SetActive(false);
        ppv = gameObject.GetComponent<Volume>();
    }

    private void FixedUpdate()
    {
        //CalcTime(); // unrem for day night [Bugged] cycle.
    }

    void CalcTime() // Used to calculate sec, min and hours
    {
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick

        if (seconds >= 60) // 60 sec = 1 min
        {
            seconds = 0;
            mins += 1;
        }

        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }
        ControlPPV(); // changes post processing volume after calculation
    }

    void ControlPPV()
    {
        ppv.weight = (float)mins / 60; // since dusk is 1 hr, we just divide the mins by 60 which will slowly increase from 0 - 1

        if (activateLights == false) // if lights havent been turned on
        {
            if (mins > 45) // wait until pretty dark
            {
                for (int i = 0; i < player.currentScene.lights.Length; i++)
                {
                    player.currentScene.lights[i].SetActive(true); // turn them all on
                }
                activateLights = true;
            }
        }

        if (hours >= 6 && hours < 7) // Dawn at 6:00 / 6am    -   until 7:00 / 7am
        {
            ppv.weight = 1 - (float)mins / 60; // we minus 1 because we want it to go from 1 - 0
            if (activateLights == true) // if lights are on
            {
                if (mins > 45) // wait until pretty bright
                {
                    for (int i = 0; i < player.currentScene.lights.Length; i++)
                    {
                        player.currentScene.lights[i].SetActive(false); // shut them off
                    }
                    activateLights = false;
                }
            }
        }
    }

    public void OpenState(GameState state, TraderController trader = null)
    {
        if(state == GameState.Menu)
        {
            menu.gameObject.SetActive(true);
        }
        else if (state == GameState.Inventory)
        {
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.UpdateContents();
        }
        else if(state == GameState.Equipment)
        {
            equipmentUI.gameObject.SetActive(true);
            equipmentUI.UpdateContents();
        }
        else if(state == GameState.Shop && trader != null)
        {
            shopUI.SetTrader(trader); // Questo deve essere chiamato prima del setActive visto che quest'ultima funzione chiama anche Awake().
            shopUI.gameObject.SetActive(true);
        }
        else if(state == GameState.Quests)
        {
            questsUI.gameObject.SetActive(true);
            questsUI.UpdateContents();
        }
        else if(state == GameState.Chest)
        {

        }

        this.state = state;
    }

    private void Update()
    {
        // fast switching: inventory -> equipment -> quests -> inventory.
        if (state != GameState.FreeRoam)
            player.animator.SetFloat("Speed", 0.0f);

        if (state == GameState.FreeRoam)
        {
            player.HandleUpdate();
        }

        else if (state == GameState.Menu)
        {
            Action onBack = () =>
            {
                menu.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            menu.HandleUpdate(onBack);
        }

        else if (state == GameState.Dialogue)
            dialogueBox.HandleUpdate();

        else if (state == GameState.Quest)
            questWindow.HandleUpdate();

        else if (state == GameState.Inventory)
        {
            inventoryUI.HandleUpdate();
            if(Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("RShoulder"))
            {
                inventoryUI.gameObject.SetActive(false);
                OpenState(GameState.Equipment);
            }
        }

        else if (state == GameState.Equipment)
        {
            equipmentUI.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("RShoulder"))
            {
                equipmentUI.gameObject.SetActive(false);
                OpenState(GameState.Quests);
            }
        }

        else if (state == GameState.Shop)
            shopUI.HandleUpdate();

        else if (state == GameState.Quests)
        {
            questsUI.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("RShoulder"))
            {
                questsUI.gameObject.SetActive(false);
                OpenState(GameState.Inventory);
            }
        }

        else if(state == GameState.Enchanting)
        {
            enchantingUI.HandleUpdate();
        }
    }

    public void ShowMessage(string msg)
    {
        dialogueBox.StartDialogue(new Dialogue(new string[] { msg }), () => { });
    }

    public void ShowInfo(string text, Action onEndDialogue, float duration = 1f)
    {
        StartCoroutine(dialogueBox.InfoDialogue(new Dialogue(new string[] { text }), duration, onEndDialogue));
    }
}
