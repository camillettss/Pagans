using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsUI : MonoBehaviour
{
    [SerializeField] GameObject contents;
    [SerializeField] QuestSlotUI questSlotPrefab;
    List<QuestSlotUI> slotUIs;

    [SerializeField] UnityEngine.UI.Text DescriptionText;

    Player player;

    int Type = -1;
    int selected = 0;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        //UpdateContents();
    }

    public void UpdateContents()
    {
        foreach (Transform child in contents.transform)
            Destroy(child.gameObject);

        if (player.QuestsContainer.GetQuestsByType(Type).Count == 0)
        {
            DescriptionText.text = "No quests here";
            return;
        }

        slotUIs = new List<QuestSlotUI>();
        foreach(var quest in player.QuestsContainer.GetQuestsByType(Type))
        {
            //print($"Quest: {quest.description}");
            var slot = Instantiate(questSlotPrefab, contents.transform);
            slot.SetData(quest);

            slotUIs.Add(slot);
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i == selected)
            {
                slotUIs[i].NameTxt.color = GameController.Instance.selectedOnBookColor;
            }
            else
            {
                slotUIs[i].NameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }

        DescriptionText.text = player.QuestsContainer.GetQuestsByType(Type)[selected].description;
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(slotUIs != null && slotUIs.Count > 0)
            {
                // MISSING: start and pause quest
                player.AcceptQuest(slotUIs[selected].quest);
                gameObject.SetActive(false);
                GameController.Instance.state = GameState.FreeRoam;
            }
        }
    }
}
