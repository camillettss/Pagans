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

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            print("agg a usci");
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // See details
            questGiver.AcceptQuest();
            FindObjectOfType<Player>().UpdateQuestUI();
        }
    }
}
