using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;

    [SerializeField] UnityEngine.Experimental.Rendering.Universal.Light2D light;
    [SerializeField] float lightBrightness = 1f;
    [SerializeField] Color lightColor;
    public GameObject[] lights;

    public bool outdoor = true;

    bool IsLoaded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        if(collision.tag == "Player")
        {
            print($"entered {gameObject.name}");

            /*LoadScene();

            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }*/

            light.color = lightColor;
            light.intensity = lightBrightness;

            Player.i.currentScene = this;

            if (gameObject.name != "Midgardr")
                NotificationsUI.i.AddNotification("entered " + gameObject.name);
        }
    }

    public void LoadSceneAsMain()
    {
        LoadScene();
        foreach (var scene in connectedScenes)
            scene.LoadScene();
    }

    public void LoadScene()
    {
        if(!IsLoaded)
        {
            print(gameObject.name + " will be loaded");
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if(IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}
