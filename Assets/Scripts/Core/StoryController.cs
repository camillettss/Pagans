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
    [SerializeField] public SceneDetails Midgardr;
    [SerializeField] SceneDetails Alfheimr;
    [SerializeField] SceneDetails Svartalfheimr;
    [SerializeField] SceneDetails Niflheimr;
    [SerializeField] SceneDetails Muspellheimr;
    [SerializeField] SceneDetails Vanheimr;
    [SerializeField] SceneDetails Jotunheimr;
    [SerializeField] SceneDetails Helheimr;
    [SerializeField] SceneDetails Asgardr;
    List<SceneDetails> allScenes;

    [SerializeField] Quest CaveQuest;

    public bool FirstTime_inventory = true;
    public bool talkedWithAsbjorn = false;
    public bool firstTimeInCave = true;

    // Tutorial variables
    bool talkedWithMan = false;

    private void Awake()
    {
        allScenes = new List<SceneDetails>() { Midgardr, Alfheimr, Svartalfheimr, Niflheimr, Muspellheimr, Vanheimr, Jotunheimr, Helheimr, Asgardr };
    }

    public void hasStartedTalking(NPCController npc)
    {

    }

    public void endDialogueWith(NPCController npc)
    {
        if (npc.Name == "Ormr" && talkedWithMan == false)
            talkedWithMan = true;
    }
}
