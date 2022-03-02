using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public bool isRunningStory;
    public Quest activeQuest;

    public SceneDetails TutorialScene;

    [SerializeField] SceneDetails Midgardr;
    [SerializeField] SceneDetails Alfheimr;
    [SerializeField] SceneDetails Svartalfheimr;
    [SerializeField] SceneDetails Niflheimr;
    [SerializeField] SceneDetails Muspellheimr;
    [SerializeField] SceneDetails Vanheimr;
    [SerializeField] SceneDetails Jotunheimr;
    [SerializeField] SceneDetails Helheimr;
    [SerializeField] SceneDetails Asgardr;

    /*private void Start()
    {
        if (GameController.Instance.isFirstLaunch)
        {
            SceneManager.LoadScene("Tutorial");
        }

        else
        {
            SceneManager.LoadScene("Gameplay"); // i chunk verranno caricati con il trigger
        }
    } // rem cuz Runner.cs will choose the scene.

    public async void Activate()
    {
        Start();
    }*/

    public void hasStartedTalking(NPCController npc)
    {

    }

    public void endDialogueWith(NPCController npc)
    {
        
    }
}
