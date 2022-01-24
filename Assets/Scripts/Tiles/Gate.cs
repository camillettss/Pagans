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

    bool isOpen = false;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();

        if (isOpen)
            sp.sprite = openSprite;
        else
            sp.sprite = closedSprite;
    }

    public void Open()
    {
        isOpen = !isOpen;
        UpdateCollider();
    }

    public void Open(bool target)
    {
        isOpen = target;
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (isOpen)
        {
            if (openSprite != null)
                GetComponent<BoxCollider2D>().enabled = false;
            else
                gameObject.SetActive(false);
        }
        else
            GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Interact(Player player)
    {
        if (player.inventory.alreadyInStock(key))
            Open(true);
        else
            print("u dont have the key");
    }

    public void takeDamage(int dmg)
    {
    }

    public void ShowSignal()
    {
    }
}
