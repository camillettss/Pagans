using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Text content;
    public Animator animator;

    Queue<string> sentences;
    void Awake()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);
        GameController.Instance.state = GameState.Dialogue;
        GameController.Instance.dialogueBox.gameObject.SetActive(true);

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public IEnumerator InfoDialogue(Dialogue dialogue, float duration)
    {
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

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        var sentence = sentences.Dequeue();
        content.text = sentence;
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        GameController.Instance.state = GameState.FreeRoam;
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
