using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class Quest
{
    public bool isActive;
    public bool activeOnGet = false;

    public string title;
    public string description;

    public int experienceReward;
    public int goldReward;

    public NPCController giver;
    [ConditionalField(nameof(giver))] public string successReaction;

    public List<QuestGoal> goal;

    public void Complete()
    {
        GameController.Instance.player.experience += experienceReward;
        GameController.Instance.player.gold += goldReward;
        GameController.Instance.player.quest = null;
        GameController.Instance.player.UpdateQuestUI();
        GameController.Instance.player.QuestsContainer.OnComplete(this);

        isActive = false;

        if(successReaction != null && giver != null)
        {
            giver.dialoguesQueue.Add(new string[] {successReaction});
        }
    }
}