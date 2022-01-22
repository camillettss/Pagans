using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsUI : MonoBehaviour
{
    [SerializeField] GameObject contents;
    [SerializeField] QuestSlotUI questSlotPrefab;
    List<QuestSlotUI> slotUIs;

    [SerializeField] Text SafetyText;

    [SerializeField] Text acceptTip;
    [SerializeField] Text viewTip;

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
            SafetyText.gameObject.SetActive(true);
            SafetyText.text = "No quests here";

            acceptTip.gameObject.SetActive(false);
            viewTip.gameObject.SetActive(false);

            return;
        }

        acceptTip.gameObject.SetActive(true);
        viewTip.gameObject.SetActive(true);
        SafetyText.gameObject.SetActive(false);

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
                if (slotUIs[i].quest == player.quest)
                    acceptTip.text = "(Z) Stop";
                else
                    acceptTip.text = "(Z) Start";
            }
            else
            {
                slotUIs[i].NameTxt.color = GameController.Instance.unselectedDefaultColor;
            }
        }
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }

        var prev = selected;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if (selected != prev)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(slotUIs != null && slotUIs.Count > 0)
            {
                if(slotUIs[selected].quest != player.quest)
                {
                    player.AcceptQuest(slotUIs[selected].quest);
                    gameObject.SetActive(false);
                    GameController.Instance.state = GameState.FreeRoam;
                }
                else
                {
                    slotUIs[selected].quest.isActive = false;

                    player.quest = null;
                    player.UpdateQuestUI();

                    UpdateContents();
                }
            }
        }
    }
}
