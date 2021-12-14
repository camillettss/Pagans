using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectTypes
{
    Crafting,
    Cauldron
}

public enum CraftingType
{
    Table,
    Naval
}

public class InteractableObject : MonoBehaviour, IEntity
{
    [SerializeField] InteractableObjectTypes type;
    [MyBox.ConditionalField(nameof(type), false, InteractableObjectTypes.Crafting)] public CraftingType craftingType;
    public Transform objectSpawner;

    public void Interact(Player player)
    {
        player.activeBench = this;

        if (type == InteractableObjectTypes.Crafting)
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
