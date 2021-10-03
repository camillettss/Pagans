using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInventory : MonoBehaviour
{
    public List<Quest> quests;
    public List<Quest> completed;

    public void Add(Quest quest)
    {
        NotificationsUI.i.AddNotification("got new quest.");
        quests.Add(quest);
    }

    public void OnComplete(Quest quest)
    {
        quests.Remove(quest);
        completed.Add(quest);
    }

    public List<Quest> GetQuestsByType(GoalType type)
    {
        var res = new List<Quest>();
        foreach(var quest in quests)
        {
            if (quest.goal.goalType == type)
                res.Add(quest);
        }

        return res;
    }

    public List<Quest> GetQuestsByType(int btype)
    {
        if (btype == -1)
            return quests;

        var type = (GoalType)btype;
        var res = new List<Quest>();
        foreach (var quest in quests)
        {
            if (quest.goal.goalType == type)
                res.Add(quest);
        }

        return res;
    }
}
