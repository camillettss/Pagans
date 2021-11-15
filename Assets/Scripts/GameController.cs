using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public enum GameState { FreeRoam, Menu, Settings, Dialogue, Quest, Inventory, Equipment, Shop, Quests, Enchanting, Battle, ChoosingItem };
public class GameController : MonoBehaviour
{
    public GameObject EssentialObjectsPrefab;

    public Player player;
    [SerializeField] public MenuController menu;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] public EquipmentUI equipmentUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] QuestsUI questsUI;
    [SerializeField] EnchantingUI enchantingUI;
    [SerializeField] public Hotbar hotbar;
    [SerializeField] public SettingsUI settingsUI;
    [SerializeField] Volume ppv; // post processing volume
    [SerializeField] GameObject BattleScene;
    [SerializeField] public ChoosingUI choosingUI;
    [SerializeField] public GameObject MinimapCanvas;
    [SerializeField] public bool LaunchStory;
    [SerializeField] public doorKeyUI keyUI;
    [SerializeField] public GameObject sparksParticle;
    [SerializeField] newInventory inventory2;

    [SerializeField] bool ResetOnEnd = false;

    float tick=60, seconds, mins, hours, days = 1;
    bool activateLights;

    public DialogueManager dialogueBox;
    public QuestController questWindow;
    public NewItemUI newItemUI;

    public StoryEventHandler EvH;

    public GameObject keytip_E;
    public GameObject keytip_Z;

    // basic
    public Color selectedDefaultColor;
    public Color unselectedDefaultColor = Color.black;
    public Color selectedOnBookColor;
    // firsts
    public Color equipedDefaultColor;
    public Color equipedSelectedColor;
    // secondaries
    public Color secondaryDefaultColor;
    public Color secondarySelectedColor;

    [HideInInspector] public GameState state = GameState.FreeRoam;
    [HideInInspector] public NPCController ActiveNPC;
    [HideInInspector] public StoryController storyController;

    public static GameController Instance;
    public EnchantingUI EnchUI => enchantingUI;

    private void Awake()
    {
        Instance = this;
        storyController = GetComponent<StoryController>();
    }

    private void Start()
    {
        //hotbar.UpdateItems();
        MinimapCanvas.SetActive(false);
        ppv = gameObject.GetComponent<Volume>();
        EvH = FindObjectOfType<StoryEventHandler>();
        player.Load();
        if(storyController.firstLaunch)
        {
            ShowMessage("benvenuto.");
        }
    }

    private void OnDestroy()
    {
        print("destroying");
        player.Save();
        /*if (ResetOnEnd)
            SaveSystem.Reset();
        else
            player.Save();*/
    }

    private void FixedUpdate()
    {
        //CalcTime(); // unrem for day night [Bugged] cycle.
    }

    #region daynight
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
    #endregion

    public void OpenState(GameState state, TraderController trader = null)
    {
        #region state control
        if(state == GameState.Menu)
        {
            menu.gameObject.SetActive(true);
        }
        else if (state == GameState.Inventory)
        {
            inventory2.UpdateView();
            inventory2.gameObject.SetActive(true);
            /*inventoryUI.gameObject.SetActive(true);
            inventoryUI.UpdateContents();
            StartCoroutine(storyController.FirstInventoryOpen());*/
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
        else if(state == GameState.ChoosingItem)
        {
            choosingUI.gameObject.SetActive(true);
            choosingUI.UpdateItems();
            choosingUI.GetComponent<Animator>().SetTrigger("Anim");
        }
        else if(state == GameState.Settings)
        {
            settingsUI.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            settingsUI.UpdateSelection();
        }
        #endregion

        this.state = state;
    }

    #region battle stuffs
    public void StartBattle()
    {
        state = GameState.Battle;
        player.transform.GetChild(0).GetComponent<Camera>().enabled = false;
        player.gameObject.SetActive(false);
        BattleScene.gameObject.SetActive(true);
    }

    public void EndBattle()
    {
        BattleScene.gameObject.SetActive(false);
        player.transform.GetChild(0).GetComponent<Camera>().enabled = true;
        player.gameObject.SetActive(true);
        state = GameState.FreeRoam;
    }
    #endregion

    private void Update()
    {
        if (state != GameState.FreeRoam)
        {
            player.animator.SetFloat("Speed", 0.0f);
            player.moveInput = Vector2.zero;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.canShowMinimap = false;
        }
        #region update choose
        // fast switching: inventory -> equipment -> quests -> inventory.

        if (state == GameState.FreeRoam)
        {
            if (!player.canShowMinimap)
                player.canShowMinimap = true;

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
            /*inventoryUI.HandleUpdate();
            if(Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("RShoulder"))
            {
                inventoryUI.gameObject.SetActive(false);
                OpenState(GameState.Equipment);
            }*/
            inventory2.HandleUpdate();
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

        else if(state == GameState.ChoosingItem)
        {
            choosingUI.HandleUpdate();
        }

        else if(state == GameState.Enchanting)
        {
            enchantingUI.HandleUpdate();
        }

        else if(state == GameState.Settings)
        {
            Action onBack = () =>
            {
                settingsUI.gameObject.SetActive(false);
                OpenState(GameState.Menu);
            };
            settingsUI.HandleUpdate(onBack);
        }
        #endregion
    }

    public void ShowMessage(string msg)
    {
        var prevState = state;
        dialogueBox.StartDialogue(new Dialogue(new string[] { msg }), () => { state = prevState; });
    }

    public void ShowMessage(string[] msgs)
    {
        var prevState = state;
        dialogueBox.StartDialogue(new Dialogue(msgs), () => { state = prevState; });
    }

    public void ShowInfo(string text, Action onEndDialogue, float duration = 1f)
    {
        StartCoroutine(dialogueBox.InfoDialogue(new Dialogue(new string[] { text }), duration, onEndDialogue));
    }
}
