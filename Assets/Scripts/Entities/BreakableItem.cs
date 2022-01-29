using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableItem : MonoBehaviour, IEntity
{
    [SerializeField] Sprite brokenSprite;

    public void Interact(Player player)
    {
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
        Break();
    }

    void Break()
    {
        if(brokenSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = brokenSprite;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
