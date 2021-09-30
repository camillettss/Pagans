using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderController : MonoBehaviour, IEntity
{
    public string Name;

    [HideInInspector] public Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public void Interact(Player player)
    {
        GameController.Instance.OpenState(GameState.Shop, this);
    }

    public void takeDamage(int dmg)
    {
        print("scemo demmerda");
    }
}
