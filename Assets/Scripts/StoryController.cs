using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public bool firstLaunch = true;
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
    List<SceneDetails> allScenes;

    private void Awake()
    {
        allScenes = new List<SceneDetails>() { Midgardr, Alfheimr, Svartalfheimr, Niflheimr, Muspellheimr, Vanheimr, Jotunheimr, Helheimr, Asgardr };
    }

    public void onLaunch()
    {
        //Midgardr.LoadSceneAsMain();
        /*var data = SaveSystem.LoadPlayer();
        if(data == null)
        {
            firstLaunch = true;
        }
        else
        {
            firstLaunch = data.firstLaunch;
        }
        if(firstLaunch)
        {
            TutorialScene.LoadSceneAsMain();
            //Player.i.Save(); // crea uno slot di salvataggio
        }
        else
        {
            Midgardr.LoadSceneAsMain();
            //Player.i.Load();
        }*/
    }
}
