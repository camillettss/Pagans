using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Gate : MonoBehaviour, IEntity
{
    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openSprite;
    [SerializeField] Key key;

    SpriteRenderer sp;

    [SerializeField] bool isOpen = false;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();

        UpdateCollider();
    }

    public void Open()
    {
        print("toggling");
        isOpen = !isOpen;
        UpdateCollider();
    }

    public void Open(bool target)
    {
        isOpen = target;
        UpdateCollider();
    }

    void UpdateCollider() // automatically updates sprite
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (isOpen)
        {
            if (openSprite != null)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                sp.sprite = openSprite;
            }
            else
                gameObject.SetActive(false);
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;
            sp.sprite = closedSprite;
        }
    }

    public void Interact(Player player)
    {
        print("yo u are interacting with me");
        if (key != null)
        {
            if (player.inventory.alreadyInStock(key))
                Open(true);
            else
                print("u dont have the key");
        }
        else
            Open();
    }

    public void takeDamage(int dmg)
    {
    }

    public void ShowSignal()
    {
    }
}
