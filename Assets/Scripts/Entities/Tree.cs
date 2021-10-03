using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IEntity
{
    [SerializeField] int hp = 5;

    [SerializeField] Sprite TreeSprite;
    [SerializeField] Sprite CuttedSprite;

    [SerializeField] List<ItemBase> drop = new List<ItemBase>();

    bool check = false;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = TreeSprite;
    }

    public void ShowSignal()
    {

    }

    public void Interact(Player player)
    {
        
    }

    private void Update()
    {
        if (GameController.Instance.state != GameState.FreeRoam)
        {
            return;
        }

        if (hp<=0 && !check)
        {
            GetComponent<SpriteRenderer>().sprite = CuttedSprite;

            foreach(var item in drop)
                Player.i.inventory.Add(item);
            
            check = true;
        }
    }

    public void takeDamage(int dmg)
    {
        hp -= dmg;
    }
}
