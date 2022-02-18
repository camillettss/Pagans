using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum DialogueTypes
{
    Std,
    Question
}

public class DialogueManager : MonoBehaviour, UIController
{
    [SerializeField] Text content;
    public Animator animator;

    System.Action afterDialogue;

    System.Action onOption1;
    System.Action onOption2;

    Queue<string> sentences;

    [SerializeField] Text option1;
    [SerializeField] Text option2;

    bool dialogueState = true;
    bool isWriting = false;

    bool skip = false;

    int selected;

    void Awake()
    {
        sentences = new Queue<string>();
    }
    private void OnEnable()
    {
        Player.i.playerInput.actions["Submit"].performed += onSubmit;
    }

    private void OnDisable() // DONT FORGET THIS MF
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
    }

    DialogueTypes actualType = DialogueTypes.Std;

    void resetUIElements()
    {
        option1.gameObject.SetActive(false);
        option2.gameObject.SetActive(false);
        content.gameObject.SetActive(true);

        dialogueState = true;
        selected = 0;
    }

    public void StartDialogue(Dialogue dialogue, System.Action onEndDialogue)
    {
        resetUIElements();

        actualType = DialogueTypes.Std;

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

        StartCoroutine(DisplayNextSentence());
    }

    public IEnumerator InfoDialogue(Dialogue dialogue, float duration, System.Action onEnd)
    {
        resetUIElements();
        afterDialogue = onEnd;
        animator.SetBool("isOpen", true);
        GameController.Instance.dialogueBox.gameObject.SetActive(true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        yield return StartCoroutine(DisplayNextSentence());
        yield return new WaitForSeconds(duration);
        EndDialogue();
    }

    public IEnumerator DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            if (actualType == DialogueTypes.Std)
                EndDialogue();

            else if (actualType == DialogueTypes.Question)
                EndQuestion();
        }
        else
        {
            content.text = "";
            var sentence = sentences.Dequeue();
            int i = 0;
            isWriting = true;
            while(i < sentence.Length - 1)
            {
                if(skip)
                {
                    content.text = sentence;
                    skip = false;
                    break;
                }
                content.text += sentence[i];
                i++;
                yield return new WaitForSeconds(.025f);
            }
            isWriting = false;
        }
    }

    public void StartQuestionDialogue(Dialogue dialogue, string op1, string op2, System.Action onOp1, System.Action onOp2)
    {
        resetUIElements();

        actualType = DialogueTypes.Question;

        option1.text = op1;
        option2.text = op2;

        onOption1 = onOp1;
        onOption2 = onOp2;

        afterDialogue = () => { };

        animator.SetBool("isOpen", true);
        // appena esegue la riga sopra esce dalla funzione ._. FIXME // ORA FUNZIONA E NON HO CAMBIATO NULLA UNITY ERA BUGGATO PORCODDIO

        GameController.Instance.state = GameState.Dialogue;
        GameController.Instance.dialogueBox.gameObject.SetActive(true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        StartCoroutine(DisplayNextSentence());
    }

    void EndQuestion()
    {
        // mostra le opzioni
        option1.gameObject.SetActive(true);
        option2.gameObject.SetActive(true);
        content.gameObject.SetActive(false);

        dialogueState = false;
        UpdateSelection();
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        afterDialogue?.Invoke();
        if(GameController.Instance.ActiveNPC != null)
        {
            GameController.Instance.ActiveNPC.canMove = true;
            GameController.Instance.ActiveNPC = null;
        }
        
    }

    void handleDialogue()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isWriting)
                StartCoroutine(DisplayNextSentence());
            else
                skip = true;
        }
    }

    void handleQuestion()
    {
        var prev = selected;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selected;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selected;

        if (selected > 1)
            selected = 0;
        else if (selected < 0)
            selected = 1;

        if (prev != selected)
            UpdateSelection();

        if(Input.GetKeyDown(KeyCode.Z))
            Perform();
    }

    void UpdateSelection()
    {
        option1.color = selected == 0 ? GameController.Instance.selectedDefaultColor : GameController.Instance.unselectedDefaultColor;
        option2.color = selected == 1 ? GameController.Instance.selectedDefaultColor : GameController.Instance.unselectedDefaultColor;
    }

    void Perform()
    {
        if (selected == 0)
            onOption1?.Invoke();
        else
            onOption2?.Invoke();

        EndDialogue();
    }

    public void HandleUpdate()
    {
        if (dialogueState)
            handleDialogue();
        else
            handleQuestion();
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        if(dialogueState)
        {
            if (!isWriting)
                StartCoroutine(DisplayNextSentence());
            else
                skip = true;
        }
        else
        {
            Perform();
        }
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        if(!dialogueState)
        {

        }
    }
}
