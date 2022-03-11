using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public enum GameState {
    FreeRoam,
    Menu, 
    Settings,
    Dialogue, 
    Quest, 
    Inventory,
    Equipment, 
    Shop, 
    Quests, 
    Enchanting,
    Battle, 
    ChoosingItem,
    Library,
    GameOver,
    Cauldron,
    CraftUI,
    Workbench,
    Port,
    Calendar,
    ChooseLanguage
};

public enum ScanMode
{
    Viewport,
    Every,
    Scene
}

public class GameController : MonoBehaviour
{
    public GameObject EssentialObjectsPrefab;

    public Player player;
    [SerializeField] public MenuController menu;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] public ShopUI shopUI;
    [SerializeField] QuestsUI questsUI;
    [SerializeField] EnchantingUI enchantingUI;
    [SerializeField] public Hotbar hotbar;
    [SerializeField] public SettingsUI settingsUI;
    [SerializeField] Volume ppv; // post processing volume
    [SerializeField] public ChoosingUI choosingUI;
    [SerializeField] public GameObject MinimapCanvas;
    [SerializeField] public bool LaunchStory;
    [SerializeField] public doorKeyUI keyUI;
    [SerializeField] public GameObject sparksParticle;
    [SerializeField] public newInventory inventory2;
    [SerializeField] public LibUI libUI;
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] public CauldronUI cauldronUI;
    public CraftUI craftUI;
    public WorkbenchUI workbenchUI;
    public PortMapUI portMapUI;
    public GameObject portTip;
    public LanguageContentController languageContentController;
    public Calendar calendar;
    public PlantDetails plantDetailsUI;
    public UnityEngine.UI.Text HourTextUI;
    public ExtraItemUI extraItemUI;

    public AudioClip outdoorBackgroundTrack;
    public AudioClip indoorBackgroundTrack;

    [SerializeField] bool ResetOnEnd = false;

    [SerializeField, Range(0, 59)]  public float mins = 55;
    [SerializeField, Range(0, 24)] public int hours = 18;
    [SerializeField, Range(0, 31)] public int day;
    [SerializeField, Range(0, 12)] public int month;
    public int year = 1248;
    public float timeMultiplier = 2f;
    bool activateLights;
    public bool canHandleLight = true;

    public DialogueManager dialogueBox;
    public QuestController questWindow;
    public NewItemUI newItemUI;
    public SacrificeUI sacrificeUI;

    public StoryEventHandler EvH;

    public GameObject keytip_E;
    public GameObject keytip_Z;
    public GameObject keytip_F;

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
    // UI
    public Color AffordableGreenColor;
    public Color UnaffordableRedColor;
    // calendar
    public Color selectedDayDefaultColor;
    public Color selectedActualDayColor;
    public Color unselectedActualDayColor;
    public Color unselectedDayDefaultColor;

    public string lid = "en";

    public GameState state = GameState.FreeRoam;
    [HideInInspector] public NPCController ActiveNPC;
    [HideInInspector] public StoryController storyController;
    public List<FarmAnimal> babyAnimals;

    [SerializeField] Quest activateAfterTutorialQuest;

    [SerializeField] Color eveningLightsColor;
    [SerializeField] Gradient nightLightsColor;

    public bool isFirstLaunch = false;

    public UnityEngine.Experimental.Rendering.Universal.Light2D GameplayLight;

    public static GameController Instance;

    public AudioSource audioSource;

    public EnchantingUI EnchUI => enchantingUI;

    public GameState prevState;

    private void Awake()
    {
        Instance = this;
        player.Load();

        storyController = GetComponent<StoryController>();
        ppv = gameObject.GetComponent<Volume>();
        EvH = GetComponent<StoryEventHandler>();
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        //hotbar.UpdateItems();
        MinimapCanvas.SetActive(false);
        Camera.main.transform.position = player.transform.position;
        hours = 5;
        mins = 55;
    }

    private void OnDestroy()
    {
        player.Save();
        //SaveSystem.Reset();
        /*if (ResetOnEnd)
            SaveSystem.Reset();
        else
            player.Save();*/
    }

    public void OpenState(GameState state, TraderController trader = null, Cauldron c = null, ItemBase craftItem = null)
    {
        //print($"target state:{state}, trader passed:{trader}.");
        IdleAllEnemies();

        prevState = this.state;

        #region state control
        
        if (state == GameState.Menu)
        {
            menu.gameObject.SetActive(true);
        }
        else if (state == GameState.Inventory)
        {
            inventory2.UpdateView();
            inventory2.gameObject.SetActive(true);
        }
        else if (state == GameState.Shop && trader != null)
        {
            shopUI.SetTrader(trader); // Questo deve essere chiamato prima del setActive visto che quest'ultima funzione chiama anche Awake().
            shopUI.gameObject.SetActive(true);
        }
        else if (state == GameState.Quests)
        {
            questsUI.gameObject.SetActive(true);
            questsUI.UpdateContents();
        }
        else if(state == GameState.Quest)
        {
            questWindow.gameObject.SetActive(true);
        }
        else if (state == GameState.ChoosingItem)
        {
            choosingUI.gameObject.SetActive(true);
            choosingUI.UpdateItems();
            choosingUI.GetComponent<Animator>().SetTrigger("Anim");
        }
        else if (state == GameState.Settings)
        {
            settingsUI.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            settingsUI.UpdateSelection();
        }
        else if (state == GameState.Library)
        {
            libUI.gameObject.SetActive(true);
            libUI.UpdateCategorySelection();
        }
        else if (state == GameState.GameOver)
        {
            gameOverUI.gameObject.SetActive(true);
        }
        else if (state == GameState.Cauldron)
        {
            cauldronUI.SetSource(c);
            cauldronUI.UpdateContents();
            cauldronUI.gameObject.SetActive(true);
        }
        else if (state == GameState.CraftUI)
        {
            craftUI.UpdateContents(craftItem);
            craftUI.gameObject.SetActive(true);
        }
        else if (state == GameState.Workbench)
        {
            workbenchUI.gameObject.SetActive(true);
            workbenchUI.UpdateContents();
        }
        else if (state == GameState.Port)
        {
            portMapUI.gameObject.SetActive(true);
            portMapUI.UpdateSelection();
        }
        else if (state == GameState.Calendar)
        {
            calendar.gameObject.SetActive(true);
            calendar.UpdateContents();
        }
        else if(state == GameState.ChooseLanguage)
        {
            // automatically did in LangContentController.Activate
            // just set the state.
        }
        else
            print("[!!] No state specified or unhandled option.");
        #endregion
        print($"overriding {this.state} with {state}");
        this.state = state;
    }

    private void Update()
    {
        UpdateTime();
        if (state == GameState.FreeRoam)
        {
            if (!player.canShowMinimap)
                player.canShowMinimap = true;

            UpdateEnemiesInViewport();
            player.HandleUpdate();
        }
        else
        {
            player.animator.SetFloat("Speed", 0.0f);
            player.moveInput = Vector2.zero;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.canShowMinimap = false;
        }
    }

    // day/night cycle
    void UpdateTime()
    {
        mins += timeMultiplier*Time.deltaTime;
        if (mins >= 60)
        {
            mins = 0;
            hours++;
            
            foreach(var ai in ScanNPCs(ScanMode.Every))
            {
                ai.AtHour(hours);
            }

            if (hours >= 24)
            {
                hours = 0;
                calendar.newDay();
                StartCoroutine(AgricultureUpdate()); // aggiorna le piante
            }
        }

        /*if (hours == 19f)
        {
            foreach(var ai in ScanNPCs(ScanMode.Scene))
            {
                ai.GoHome();
            }
        }*/

        var scaleTime = hours + (((10 * mins) / 6) / 100);

        try
        {
            if (player.currentScene.outdoor && canHandleLight)
            {
                GameplayLight.color = nightLightsColor.Evaluate(hours / 24);
                // example: 22h 30m = 22.5f
                GameplayLight.intensity = Mathf.Clamp((Mathf.Sin((scaleTime / 3.8f) - 1.58f) + 1.2f) / 2, 0.1f, 1);
            }
        }
        catch
        {

        }

        HourTextUI.text = $"{hours}:{(int)mins}";

    }

    List<NoAIBehaviour> ScanNPCs(ScanMode mode)
    {
        if (mode==ScanMode.Every)
        {
            List<NoAIBehaviour> res = new List<NoAIBehaviour>();
            foreach (var obj in FindObjectsOfType<NoAIBehaviour>())
                res.Add(obj);
            return res; // hope this workls everywhere.
        }
        else if(mode == ScanMode.Scene)
        {
            List<NoAIBehaviour> res = new List<NoAIBehaviour>();
            foreach (Transform child in player.triggeredCity.entitiesContainer.transform)
                if (child.TryGetComponent(out NoAIBehaviour npc))
                    res.Add(npc);
            return res;
        }
        else if(mode == ScanMode.Viewport)
        {
            throw new NotImplementedException("la scansione per viewport non è ancora stata programmata");
        }
        return new List<NoAIBehaviour>();
    }

    void UpdateEnemiesInViewport() // this shouldn't be used, just create chunks!!
    {
        foreach(EnemyController npc in FindObjectsOfType<EnemyController>())
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(npc.transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
            {
                npc.HandleUpdate(); // update only if is visible.
            }
        }
    }

    public IEnumerator Sleep(Action onwakeup)
    {
        player.canMove = false;
        yield return Fader.i.FadeIn(1f);

        if(hours >= 20)
        {
            calendar.newDay();
            StartCoroutine(AgricultureUpdate());
        }

        hours = 7;
        mins = 0;

        yield return Fader.i.FadeOut(1f);
        player.canMove = true;

        onwakeup?.Invoke();
    }

    public IEnumerator AgricultureUpdate() // comprende anche la crescita degli animali
    {
        foreach(var plant in AgribleTile.Instances)
        {
            plant.NewDay();
        }
        foreach(var animal in babyAnimals)
        {
            animal.newDay();
        }
        yield return null;

    }

    void IdleAllEnemies() // this is heavy porcoddio 
    {
        foreach(EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.GetComponent<Animator>().SetFloat("Speed", 0f);
        }
    }

    public void ShowMessage(string msg)
    {
        ShowMessage(new string[] { msg });
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