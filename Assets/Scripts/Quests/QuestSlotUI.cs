using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSlotUI : MonoBehaviour
{
    [SerializeField] public UnityEngine.UI.Text NameTxt;
    [HideInInspector] public Quest quest;
    public void SetData(Quest quest)
    {
        this.quest = quest;
        NameTxt.text = quest.title.GetLocalizedString();
    }
}
