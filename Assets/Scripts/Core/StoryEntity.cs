using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class StoryEntity : MonoBehaviour
{
    public void onStartTalk()
    {
        GameController.Instance.storyController.hasStartedTalking(GetComponent<NPCController>());
    }

    public void onEndTalk()
    {
        GameController.Instance.storyController.endDialogueWith(GetComponent<NPCController>());
    }
}
