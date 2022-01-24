using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsController : MonoBehaviour
{
    [SerializeField] Quest killTwo_Quest;

    [SerializeField] Gate level3gate;
    [SerializeField] Gate level1gate;

    public void triggered(TutorialRoom room)
    {
        if (room.alreadyEntered)
            return;
        else room.alreadyEntered = true;

        if(room.level == 1)
        {
            StartCoroutine(StartDialogue(new string[] { "Ciao, tu devi essere Njal.",
                "Io sono Ulfr e ti guiderò alla scoperta di Pagans.",
            "Esci da questo sotterraneo così potrai finalmente vedere la luce."}));
        }
        else if(room.level == 2)
        {
            level1gate.Open(false);
            StartCoroutine(
                StartDialogue(new string[]
            {
                "Dovrai uccidere quei goblin per aprire la porta ma attento, ti attaccheranno se ti avvicini troppo."
            }, .2f, ()=>
            {
                Player.i.QuestsContainer.Add(killTwo_Quest, () =>
                {
                    level3gate.Open(true);
                    StartCoroutine(StartDialogue(new string[]
                    {
                        "ottimo! ora entra in quella stanza ed apri il forziere."
                    }));
                });
            }));
        }
        else if(room.level == 3)
        {
            
        }
        else if(room.level == 4)
        {
            StartCoroutine(StartDialogue(new string[]
            {
                "Hai completato il tutorial, complimenti!",
                "Purtroppo questa è solo una demo e dovrai aspettare per partire verso Asgard.",
                "Esci da questi sotterranei e goditi questo piccolo mondo finchè non uscirà la versione definitiva!"
            }));
        }
        else if(room.level == 5)
        {
            // carica TutorialMain
        }
    }

    IEnumerator StartDialogue(string[] sentences, float timer = 0f, System.Action after = default)
    {
        yield return new WaitForSeconds(timer);
        GameController.Instance.dialogueBox.StartDialogue(new Dialogue(sentences), () =>
        {
            after?.Invoke();
            GameController.Instance.state = GameState.FreeRoam;
        });
    }

    public void exits(TutorialRoom room)
    {
        if (room.alreadyLeft)
            return;
        else room.alreadyLeft = true;

        if(room.level == 3)
        {
            StartCoroutine(StartDialogue(new string[]
            {
                "Ora che hai la chiave puoi aprire quella porta!"
            }));
        }
    }

}
