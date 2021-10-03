using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IEntity
{
    [SerializeField] List<ItemBase> drop;

    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openSprite;
    [SerializeField] GameObject signal;

    [SerializeField] Animator animator;

    [SerializeField] Sprite background;

    bool isOpen = false;

    public void Interact(Player player)
    {
        if (isOpen)
            return;

        isOpen = true;
        animator.SetTrigger("Open");
        animator.SetBool("isOpen", true);

        foreach(var item in drop)
        {
            player.inventory.Add(item);
        }
    }

    public void ShowSignal()
    {
        if (!signal.activeSelf && !isOpen)
            signal.gameObject.SetActive(true);
    }
    void unShowSignal()
    {
        if (signal.activeSelf)
            signal.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if(GameController.Instance.state != GameState.FreeRoam)
        {
            return;
        }

        if (FindObjectOfType<Player>().isInRange(this))
        {
            ShowSignal();
        }
        else
            unShowSignal();

        if (isOpen && signal.activeSelf)
            signal.SetActive(false);

        if (isOpen && GetComponent<SpriteRenderer>().sprite != openSprite)
            GetComponent<SpriteRenderer>().sprite = openSprite;

        else if (!isOpen && GetComponent<SpriteRenderer>().sprite != closedSprite)
            GetComponent<SpriteRenderer>().sprite = closedSprite;
    }

    public void takeDamage(int dmg)
    {

    }
}
