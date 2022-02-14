using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public Text titleText;
    public Text DescText;
    public Text goldText;
    public NPCController questGiver;

    public void Open(Quest quest)
    {
        GameController.Instance.OpenState(GameState.Quest);

        titleText.text = quest.title.GetLocalizedString();
        DescText.text = quest.description.GetLocalizedString();
        goldText.text = $"{quest.goldReward}";

        questGiver = quest.giver;
    }

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            print("agg a usci");
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.Quests;
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // See details
            questGiver.AcceptQuest();
            FindObjectOfType<Player>().UpdateQuestUI();
        }*/
    }
}
