using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public bool firstLaunch;
    public bool isRunningStory;
    public Quest activeQuest;

    [SerializeField] SceneDetails TutorialScene;
    [SerializeField] public SceneDetails Midgardr;
    [SerializeField] SceneDetails Alfheimr;
    [SerializeField] SceneDetails Svartalfheimr;
    [SerializeField] SceneDetails Niflheimr;
    [SerializeField] SceneDetails Muspellheimr;
    [SerializeField] SceneDetails Vanheimr;
    [SerializeField] SceneDetails Jotunheimr;
    [SerializeField] SceneDetails Helheimr;
    [SerializeField] SceneDetails Asgardr;
    List<SceneDetails> allScenes;

    [SerializeField] Quest CaveQuest;

    public bool FirstTime_inventory = true;
    public bool talkedWithAsbjorn = false;
    public bool firstTimeInCave = true;

    private void Awake()
    {
        allScenes = new List<SceneDetails>() { Midgardr, Alfheimr, Svartalfheimr, Niflheimr, Muspellheimr, Vanheimr, Jotunheimr, Helheimr, Asgardr };
    }

    public void NPCTalked(NPCController npc)
    {
        if(firstLaunch)
        {
            if (npc.Name == "Asbjorn" && !npc.done)
            {
                talkedWithAsbjorn = true;
                StartCoroutine(AsbjornDialogueDone());
            }

            else if (npc.Name == "Ulfr" && !npc.done && talkedWithAsbjorn)
                StartCoroutine(UlfrDialogueDone());

            GameController.Instance.state = GameState.FreeRoam;

            if (Player.i.quest.goal != null)
                Player.i.quest.goal[0].NPCTalked(npc);

            npc.done = true;

            if (npc.Name == "Ulfr" && !talkedWithAsbjorn)
                npc.done = false;
        }
        else if(isRunningStory)
        {
            if(npc.Name == "Harbardr")
            {
                // ha parlato con harbard (ovviamente dopo il tutorial)
            }
        }
    }

    public void Launch()
    {
        Midgardr.LoadSceneAsMain();

        Player.i.Load();
        if(firstLaunch)
        {
            GameController.Instance.ShowMessage(new string[] {
                "benvenuto, questo è Pagans ed ora sei a midgardr.",
                "piccolo appunto, generalmente si usa Z per confermare e X per annullare. non dimenticarlo o sarà molto divertente.",
                "tornando a noi, quell'omino vestito di rosso si chiama Asbjorn, vai a fargli una visita e premi Z per parlargli, ha qualcosa da dirti."
            });
        }
        else
        {
            print("no first launch");
        }
    }

    public IEnumerator AsbjornDialogueDone()
    {
        yield return new WaitForSeconds(2f);
        GameController.Instance.ShowMessage(new string[]
        {
            "hai ottenuto la tua prima quest! premi Enter per visualizzare il menu e scegli Quests.",
            "seleziona la quest che intendi avviare e premi Z per iniziarla!"
        });
    }


    public IEnumerator AsbjornQuestAccepted()
    {
        yield return new WaitForSeconds(1f);
        GameController.Instance.ShowMessage("adesso segui l'obiettivo in alto a sinistra per completare la missione.");
    }

    public IEnumerator FirstInventoryOpen()
    {
        if (!FirstTime_inventory || !GameController.Instance.LaunchStory)
            yield return null;
        else
        {
            yield return new WaitForSeconds(1f);
            GameController.Instance.ShowMessage("Questo è l'inventario. qui troverai consumabili e oggetti utili.");
            FirstTime_inventory = false;
        }
    }
    public IEnumerator UlfrDialogueDone()
    {
        yield return new WaitForSeconds(.5f);
        GameController.Instance.dialogueBox.StartDialogue(new Dialogue(new string[]
        {
            "avrai molte cose da fare, purtroppo questa versione è solo una trial quindi dovrai aspettare per partire verso Asgardr.",
            "per tua fortuna c'è molto da fare anche in questa piccola città.",
            "verso le montagne a nord c'è una caverna. per raggiungerla usa la minimappa (tieni premuto Q mentre esplori)."
        }),
        () =>
        {
            Player.i.kents = 10;
            GameController.Instance.state = GameState.FreeRoam; // GameController.ShowMessage ha uno state switching automatico, qui no.

            Player.i.QuestsContainer.Add(CaveQuest);
            Player.i.AcceptQuest(CaveQuest);
        });
    }


    public IEnumerator EnteredCave()
    {
        if(firstTimeInCave)
        {
            yield return new WaitForSeconds(0.2f);
            GameController.Instance.ShowMessage(new string[]
            {
                "QUELLO E' UN ENT!",
                "gli ent sono delle creature del bosco. da grandi sono enormi alberi capaci di muoversi, ma da piccoli beh.. eccoli qua.",
                "vai nel tuo equipaggiamento, per farlo apri lo zaino e premi TAB. questo tasto serve per cambiare pagina.",
                "ora seleziona la categoria delle armi con Z. poi usa Z su un'arma per equipaggiarla come primaria ed S per farlo come secondaria.",
                "quei quadrati in basso a destra nella schermata di gioco servono a mostrarti quali armi hai impostato. premi T per scambiarli e R per usare quello equipaggiato!"
            });
            firstTimeInCave = false;
        }
        else
            yield return null;
    }

    public IEnumerator FinalDialogue()
    {
        yield return new WaitForSeconds(0.3f);
        GameController.Instance.ShowMessage(new string[]
        {
            "Ce l'hai fatta! purtroppo il tutorial finisce qui, ma ci sono moltissime altre funzionalità di gioco come il commercio, la magia, la pesca o l'agricoltura",
            "potrai scoprirle da solo, tutte quante. adesso puoi esplorare liberamente."
        });
        firstLaunch = false;
        Player.i.isFirstLaunch = false;
    }
}
