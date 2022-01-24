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
    BuyWeapon,
    EquipItem,
    BuyTot,
    SellTot,
    GetTot
}

[System.Serializable]
public class QuestGoal
{
    public float dialogueDelay = 1f;
    public Dialogue introDialogue;

    public string goal;

    public GoalType goalType;

    [ConditionalField(nameof(goalType), false, GoalType.KillSomeone)] public string enemyName;

    [ConditionalField(nameof(goalType), false, GoalType.Talk)] public string talkTo;

    [ConditionalField(nameof(goalType), false, new object[] { GoalType.Buy, GoalType.Sell })] [SerializeField] string sellerName;
    [ConditionalField(nameof(goalType), false, GoalType.Buy, GoalType.GetItem, GoalType.Sell, GoalType.BuyTot, GoalType.SellTot, GoalType.GetTot)] [SerializeField] ItemBase GoalItem;

    [ConditionalField(nameof(goalType), false, GoalType.EnterADoor)] [SerializeField] string PortalName;

    [ConditionalField(nameof(goalType), false, GoalType.EquipItem)] [SerializeField] ItemBase itemToEquip;

    [ConditionalField(nameof(goalType), false, new object[] { GoalType.BuyTot, GoalType.SellTot, GoalType.GetTot, GoalType.KillTot })] [SerializeField] int reqAmount;

    public int currentAmount = 0;

    public int RequiredAmount => reqAmount;

    void Complete()
    {
        var quest = Player.i.quest;
        if (quest.goal.Count == 1)
        {
            quest.goal.RemoveAt(0);
            quest.Complete();
            Player.i.UpdateQuestUI();
        }
        else if (quest.goal.Count >= 2)
        {
            Player.i.StartCoroutine(GameController.Instance.EvH.GoalCompleted(quest));
        }
    }

    public void EnemyKilled(NPCController enemy)
    {
        if(goalType == GoalType.KillTot)
        {
            currentAmount++;
            Player.i.UpdateQuestUI();
        }

        if (currentAmount >= reqAmount)
            Complete();

        else if (goalType == GoalType.KillSomeone)
        {
            if (enemy.Name == enemyName)
                Complete();
        }
    }

    public void EnemyKilled(EnemyController enemy)
    {
        if (goalType == GoalType.KillTot)
        {
            currentAmount++;
            Player.i.UpdateQuestUI();
        }

            if (currentAmount >= reqAmount)
            Complete();

        else if (goalType == GoalType.KillSomeone)
        {
            if (enemy.Name == enemyName)
                Complete();
        }
    }

    public void NPCTalked(NPCController npc)
    {
        if(goalType == GoalType.Talk)
        {
            if (npc.Name == talkTo)
                Complete();
        }
    }

    public void DoorEntered(Portal door) // viene passata la destinazione
    {
        if (goalType == GoalType.EnterADoor && door.name == PortalName)
            Complete();
    }

    public void SomethingBought(TraderController seller, ItemBase item, int howMuch=1)
    {
        if(goalType == GoalType.Buy && seller.Name == sellerName && item.Name == GoalItem.Name)
        {
            Complete();
        }

        if(goalType == GoalType.BuyWeapon)
        {
            if (item is Weapon)
                Complete();
        }

        else if(goalType == GoalType.BuyTot)
        {
            if (item.Name == GoalItem.Name)
            {
                currentAmount += howMuch;
                Player.i.UpdateQuestUI();
            }
            if (currentAmount == reqAmount)
                Complete();
        }
    }

    public void SomethingSelled(TraderController buyer, ItemBase merch, int howMuch=1)
    {
        Debug.Log($"something selled, passed: {buyer}, {merch}");
        if(goalType == GoalType.Sell && merch.Name == GoalItem.Name)
        {
            if (sellerName != null && sellerName != "") // seller in questo caso fa da buyer, sti cazzi del nome non mi va di fare troppe vars
            {
                if (sellerName == buyer.Name)
                    Complete();
            }
            else
                Complete();
        }
        if (goalType == GoalType.SellTot && merch.Name == GoalItem.Name)
        {
            Debug.Log($"curr:{currentAmount}, req:{reqAmount}");
            currentAmount+=howMuch;
            Player.i.UpdateQuestUI();
            if (currentAmount >= reqAmount)
                Complete();
        }
    }

    public void SomethingAddedToInventory(ItemBase addedItem)
    {
        if (goalType == GoalType.GetItem && addedItem.Name == GoalItem.Name)
            Complete();

        if(goalType == GoalType.GetTot && addedItem.Name == GoalItem.Name)
        {
            currentAmount++;
            Player.i.UpdateQuestUI();
            if (currentAmount >= reqAmount)
                Complete();
        }
    }

    public void SomethingEquiped(ItemBase item)
    {
        if(goalType == GoalType.EquipItem)
        {
            if (item.Name == itemToEquip.Name)
            {
                Complete();
            }
        }
    }
}