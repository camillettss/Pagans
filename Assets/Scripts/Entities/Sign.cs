using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, IEntity
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject signal;
    bool isRead = false;

    public void Interact(Player player)
    {
        GameController.Instance.dialogueBox.StartDialogue(dialogue, () => { isRead = true; });
    }

    public void takeDamage(int dmg)
    {

    }

    public void ShowSignal()
    {
        if (!signal.activeSelf && !isRead)
            signal.gameObject.SetActive(true);
    }
    void unShowSignal()
    {
        if (signal.activeSelf)
            signal.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.state != GameState.FreeRoam || !GameController.Instance.player.isActiveAndEnabled)
        {
            return;
        }

        if (FindObjectOfType<Player>().isInRange(this))
        {
            ShowSignal();
        }
        else
            unShowSignal();

        if (isRead && signal.activeSelf)
            signal.SetActive(false);
    }
}
