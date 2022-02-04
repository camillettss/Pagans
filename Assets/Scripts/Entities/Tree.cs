using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IEntity
{
    [SerializeField] int hp = 5;

    [SerializeField] Sprite TreeSprite;
    [SerializeField] Sprite CuttedSprite;

    [SerializeField] List<ItemBase> drop = new List<ItemBase>();

    [SerializeField] bool hasFruits = false;
    [MyBox.ConditionalField(nameof(hasFruits), false, true)] [SerializeField] ItemBase dropfruit;

    bool cutted = false;
    bool showingSignal = false;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = TreeSprite;
    }

    /*void unshowSignal()
    {
        if (showingSignal)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            showingSignal = false;
        }
    }*/

    public void ShowSignal()
    {
        /*unshowSignal(); // it must be deleted
        if (!cutted && !hasFruits)
        {
            if (Player.i.inventory.Tools[Player.i.inventory.equipedTool].item is Axe)
            {
                Instantiate(GameController.Instance.keytip_E, transform);
                showingSignal = true;
            }
        }
        else if (!cutted && hasFruits)
        {
            Instantiate(GameController.Instance.keytip_Z, transform);
            showingSignal = true;
        }*/
    }

    /*private void FixedUpdate()
    {
        if (GameController.Instance.state != GameState.FreeRoam)
        {
            unshowSignal();
            return;
        }

        if (Player.i.isInRange(this))
            ShowSignal();
        else
            unshowSignal();
    }*/

    public void Interact(Player player)
    {
        // Drop a fruit
        if(!cutted)
            StoryEventHandler.i.AddToInventory(dropfruit);
        ShowSignal();
    }

    public void Cut()
    {
        if (cutted)
            return;

        GetComponent<SpriteRenderer>().sprite = CuttedSprite;

        foreach (var item in drop)
            StoryEventHandler.i.AddToInventory(item);

        cutted = true;
        GetComponent<CircleCollider2D>().enabled = false;

        ShowSignal(); // Update signal
        
    }

    public void takeDamage(int dmg)
    {

    }
}
