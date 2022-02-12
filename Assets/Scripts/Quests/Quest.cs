using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using MyBox;

[System.Serializable]
public class Quest
{
    public bool isActive;
    public bool activeOnGet = false;

    public bool initialized = false; // used in player.updateQuestUI

    public LocalizedString title;
    public LocalizedString description;

    public int experienceReward;
    public int goldReward;

    public NPCController giver;
    [ConditionalField(nameof(giver))] public string successReaction;

    public List<QuestGoal> goal;

    public Dialogue onEndDialogue;

    public System.Action onComplete = default;

    public void Complete()
    {
        Player.i.confettis.Play();

        GameController.Instance.player.experience += experienceReward;
        GameController.Instance.player.gold += goldReward;
        GameController.Instance.player.quest = null;
        GameController.Instance.player.QuestsContainer.OnComplete(this);

        Player.i.UpdateQuestUI();

        isActive = false;

        if(successReaction != null && giver != null)
        {
            giver.dialoguesQueue.Add(new string[] {successReaction});
        }

        if (onEndDialogue.sentences.Length > 0)
        {
            var actualState = GameController.Instance.state;
            GameController.Instance.dialogueBox.StartDialogue(onEndDialogue, () =>
            {
                GameController.Instance.state = actualState;
            });
            if (title.GetLocalizedString() == "Tutorial")
                GameController.Instance.isFirstLaunch = false;
            Player.i.Save();
        }
    }
}