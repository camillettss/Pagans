using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, IEntity
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject signal;
    bool isRead = false;

    private void Awake()
    {
        isRead = false;
    }

    public void Interact(Player player)
    {
        if (dialogue.sentences.Length == 0)
            return;

        GameController.Instance.dialogueBox.StartDialogue(dialogue, () =>
        {
            isRead = true;
            GameController.Instance.state = GameState.FreeRoam;
        });
    }

    public void takeDamage(int dmg)
    {

    }

    public void ShowSignal()
    {
        if ((!isRead) && (!signal.activeSelf) && dialogue.sentences.Length > 0)
            signal.gameObject.SetActive(true);
    }

    void unShowSignal()
    {
        if (signal.activeSelf)
            signal.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        //print(name + ":" + isRead);
        if (GameController.Instance.state != GameState.FreeRoam || !Player.i.isActiveAndEnabled)
        {
            return;
        }

        if (Player.i.isInRange(this))
        {
            ShowSignal();
        }
        else
            unShowSignal();

        if (isRead && signal.activeSelf)
            signal.SetActive(false);
    }
}
