using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectTypes
{
    Crafting,
    Cauldron
}

public class InteractableObject : MonoBehaviour, IEntity
{
    [SerializeField] InteractableObjectTypes type;

    public void Interact(Player player)
    {
        if(type == InteractableObjectTypes.Crafting)
        {
            GameController.Instance.OpenState(GameState.Workbench);
        }
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
    }
}
