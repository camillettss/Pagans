using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public bool firstLaunch;
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

    bool FirstTime_inventory = true;
    public bool talkedWithAsbjorn = false;

    private void Awake()
    {
        allScenes = new List<SceneDetails>() { Midgardr, Alfheimr, Svartalfheimr, Niflheimr, Muspellheimr, Vanheimr, Jotunheimr, Helheimr, Asgardr };
    }

    public void Launch()
    {
        Midgardr.LoadSceneAsMain();

        Player.i.Load();
        if(firstLaunch)
        {
            GameController.Instance.ShowMessage(new string[] {
                "benvenuto, questo è Pagans ed ora sei a midgardr. premi Z per continuare.",
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

    public IEnumerator UlfrDialogueDone()
    {
        yield return new WaitForSeconds(.5f);
        GameController.Instance.dialogueBox.StartDialogue(new Dialogue(new string[]
        {
            "avrai molte cose da fare, purtroppo questa versione è solo una trial quindi dovrai aspettare per partire verso Asgardr.",
            "per tua fortuna c'è molto da fare anche in questa piccola città.",
            "adesso vai dal fabbro e compra qualcosa, sei libero di fare ciò che vuoi qui."
        }),
        () =>
        {
            Player.i.kents = 10;
            //Player.i.QuestsContainer.Add(new Quest("le prime armi", "il mondo non è solo rosa e fiori. bisogna imparare a combattere.", 50, 5));
            GameController.Instance.state = GameState.FreeRoam; // GameController.ShowMessage ha uno state switching automatico, qui passa sempre a Freeroam.
            Player.i.isFirstLaunch = false;
        });
    }

    public IEnumerator AsbjornQuestAccepted()
    {
        yield return new WaitForSeconds(1f);
        GameController.Instance.ShowMessage("adesso segui l'obiettivo in alto a sinistra per completare la missione.");
    }

    public IEnumerator FirstInventoryOpen()
    {
        if (!FirstTime_inventory)
            yield return null;
        else
        {
            yield return new WaitForSeconds(1f);
            GameController.Instance.ShowMessage("Questo è l'inventario. qui troverai consumabili e oggetti utili.");
            FirstTime_inventory = false;
        }
    }
}
