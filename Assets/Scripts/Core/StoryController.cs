using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public bool firstLaunch;
    public bool isRunningStory;
    public Quest activeQuest;

    [SerializeField] SceneDetails TutorialScene;
    [SerializeField] SceneDetails Midgardr;
    [SerializeField] SceneDetails Alfheimr;
    [SerializeField] SceneDetails Svartalfheimr;
    [SerializeField] SceneDetails Niflheimr;
    [SerializeField] SceneDetails Muspellheimr;
    [SerializeField] SceneDetails Vanheimr;
    [SerializeField] SceneDetails Jotunheimr;
    [SerializeField] SceneDetails Helheimr;
    [SerializeField] SceneDetails Asgardr;

    private void Awake()
    {
        if(firstLaunch)
        {
            TutorialScene.LoadSceneAsMain();
        }

        else
            Midgardr.LoadSceneAsMain();
    }

    public void hasStartedTalking(NPCController npc)
    {

    }

    public void endDialogueWith(NPCController npc)
    {
        
    }
}
