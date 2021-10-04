using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Text content;
    public Animator animator;

    System.Action afterDialogue;

    Queue<string> sentences;
    void Awake()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, System.Action onEndDialogue, GameState StateAfterDialogue=GameState.FreeRoam)
    {
        animator.SetBool("isOpen", true);
        // appena esegue la riga sopra esce dalla funzione ._. FIXME // ORA FUNZIONA E NON HO CAMBIATO NULLA UNITY ERA BUGGATO PORCODDIO

        GameController.Instance.state = GameState.Dialogue;
        GameController.Instance.dialogueBox.gameObject.SetActive(true);

        afterDialogue = onEndDialogue;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence(StateAfterDialogue);
    }

    public IEnumerator InfoDialogue(Dialogue dialogue, float duration, System.Action onEnd)
    {
        afterDialogue = onEnd;
        animator.SetBool("isOpen", true);
        GameController.Instance.dialogueBox.gameObject.SetActive(true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
        yield return new WaitForSeconds(duration);
        EndDialogue();
    }

    public void DisplayNextSentence(GameState tstate=GameState.FreeRoam)
    {
        if(sentences.Count == 0)
        {
            EndDialogue(tstate);
            return;
        }

        var sentence = sentences.Dequeue();
        content.text = sentence;
    }

    void EndDialogue(GameState targetState=GameState.FreeRoam)
    {
        animator.SetBool("isOpen", false);
        afterDialogue?.Invoke();
        GameController.Instance.state = targetState;
        if(GameController.Instance.ActiveNPC != null)
            GameController.Instance.ActiveNPC.onTalk();
        GameController.Instance.ActiveNPC = null;
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            DisplayNextSentence();
        }
    }
}
