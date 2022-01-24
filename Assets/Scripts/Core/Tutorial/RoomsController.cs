using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsController : MonoBehaviour
{
    [SerializeField] Quest killTwo_Quest;

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
        if(room.level == 2)
        {
            StartCoroutine(
                StartDialogue(new string[]
            {
                "Dovrai uccidere quei goblin per aprire la porta ma attento, ti attaccheranno se ti avvicini troppo."
            }, .8f, ()=>
            {
                Player.i.QuestsContainer.Add(killTwo_Quest);
            }));
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

    }

}
