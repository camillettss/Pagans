using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInventory : MonoBehaviour
{
    public List<Quest> quests;
    public List<Quest> completed;

    public void Add(Quest quest, System.Action onComplete=default)
    {
        NotificationsUI.i.AddNotification("got new quest.");
        quest.onComplete = onComplete;
        quests.Add(quest);
        if (quest.activeOnGet)
            Player.i.AcceptQuest(quest);
    }

    public void OnComplete(Quest quest)
    {
        quests.Remove(quest);
        completed.Add(quest);
        quest.onComplete?.Invoke();
    }

    public List<Quest> GetQuestsByType(GoalType type)
    {
        var res = new List<Quest>();
        foreach(var quest in quests)
        {
            if (quest.goal[0].goalType == type)
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
            if (quest.goal[0].goalType == type)
                res.Add(quest);
        }

        return res;
    }
}
