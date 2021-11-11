﻿using UnityEngine;
using MyBox;

[System.Serializable]
public class QuestGoal
{
    public string goal;

    public GoalType goalType;

    bool done = false;

    [ConditionalField(nameof(goalType), false, GoalType.KillTot)] public int requiredAmount;
    [ConditionalField(nameof(goalType), false, GoalType.KillTot)] public int currentAmount;

    [ConditionalField(nameof(goalType), false, GoalType.KillSomeone)] public string enemyName;

    [ConditionalField(nameof(goalType), false, GoalType.Talk)] public string talkTo;

    [ConditionalField(nameof(goalType), false, GoalType.Buy)] [SerializeField] string sellerName;
    [ConditionalField(nameof(goalType), false, GoalType.Buy)] [SerializeField] string itemName;

    [ConditionalField(nameof(goalType), false, GoalType.EnterADoor)] [SerializeField] string PortalName;

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

            //if (enemy.Name == "Ent1")
            //    GameController.Instance.StartCoroutine(GameController.Instance.storyController.FinalDialogue());
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

    public void DoorEntered(Portal door)
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
    }
}

public enum GoalType
{
    KillTot,
    KillSomeone,
    Talk,
    Buy,
    EnterADoor
}