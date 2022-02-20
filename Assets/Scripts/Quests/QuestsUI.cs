using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestsUI : MonoBehaviour, UIController
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

    private void OnDisable()
    {
        player.playerInput.actions["Submit"].performed -= onSubmit;
        player.playerInput.actions["Navigate"].performed -= onNavigate;
        player.playerInput.actions["Cancel"].performed -= onCancel;
        player.playerInput.SwitchCurrentActionMap("Player");
    }

    private void OnEnable()
    {
        if(player.playerInput.currentActionMap.name != "UI")
            Player.i.playerInput.SwitchCurrentActionMap("UI");

        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        if (slotUIs != null && slotUIs.Count > 0)
        {
            if (slotUIs[selected].quest != player.quest)
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

    public void onCancel(InputAction.CallbackContext ctx)
    {
        GameController.Instance.state = GameState.FreeRoam;
        gameObject.SetActive(false);
        player.playerInput.SwitchCurrentActionMap("Player");
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>().y;

        if (input < 0) selected++;
        else if(input > 0) selected--;

        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        UpdateSelection();
    }
}
