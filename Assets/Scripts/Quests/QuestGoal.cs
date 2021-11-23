using UnityEngine;
using MyBox;

public enum GoalType
{
    KillTot,
    KillSomeone,
    Talk,
    Buy,
    Sell,
    EnterADoor,
    GetItem,
    BuyWeapon
}

[System.Serializable]
public class QuestGoal
{
    public float dialogueDelay = 1f;
    public Dialogue introDialogue;

    public string goal;

    public GoalType goalType;

    bool done = false;

    [ConditionalField(nameof(goalType), false, GoalType.KillTot)] public int requiredAmount;
    [ConditionalField(nameof(goalType), false, GoalType.KillTot)] public int currentAmount;

    [ConditionalField(nameof(goalType), false, GoalType.KillSomeone)] public string enemyName;

    [ConditionalField(nameof(goalType), false, GoalType.Talk)] public string talkTo;

    [ConditionalField(nameof(goalType), false, new object[] { GoalType.Buy, GoalType.Sell })] [SerializeField] string sellerName;
    [ConditionalField(nameof(goalType), false, GoalType.Buy)] [SerializeField] string itemName;

    [ConditionalField(nameof(goalType), false, GoalType.EnterADoor)] [SerializeField] string PortalName;

    [ConditionalField(nameof(goalType), false, new object[] { GoalType.GetItem, GoalType.Sell })] [SerializeField] ItemBase GoalItem;

    [ConditionalField(nameof(goalType), false, GoalType.BuyWeapon)] [SerializeField] ItemBase objForType;

    public bool isReached()
    {
        if (goalType == GoalType.KillTot)
            return (currentAmount >= requiredAmount && requiredAmount != 0);

        else
            return done;
    }

    public void EnemyKilled(NPCController enemy)
    {
        if(goalType == GoalType.KillTot)
            currentAmount++;
        else if(goalType == GoalType.KillSomeone)
        {
            if (enemy.Name == enemyName)
                done = true;
        }
    }

    public void NPCTalked(NPCController npc)
    {
        if(goalType == GoalType.Talk)
        {
            if (npc.Name == talkTo)
                done = true;
            
        }
    }

    public void DoorEntered(Portal door) // viene passata la destinazione
    {
        if (door.name == PortalName)
            done = true;
    }

    public void SomethingBought(TraderController seller, ItemBase item)
    {
        if(seller.Name == sellerName && item.Name == itemName)
        {
            done = true;
        }

        if(goalType == GoalType.BuyWeapon)
        {
            if (item is Weapon)
                done = true;
        }
    }

    public void SomethingSelled(TraderController buyer, ItemBase merch)
    {
        if(merch.Name == GoalItem.Name)
        {
            if (sellerName != null && sellerName != "") // seller in questo caso fa da buyer, sti cazzi del nome non mi va di fare troppe vars
            {
                if (sellerName == buyer.Name)
                    done = true;
            }
            else
                done = true;
        }
    }

    public void SomethingAddedToInventory(ItemBase addedItem)
    {
        if (addedItem.Name == GoalItem.Name)
            done = true;
    }
}