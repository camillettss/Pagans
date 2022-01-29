using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;

    [SerializeField] public UnityEngine.Experimental.Rendering.Universal.Light2D light;
    [SerializeField] float lightBrightness = 1f;
    [SerializeField] Color lightColor;
    public GameObject[] lights;

    public bool outdoor = true;
    public Vector2 door; // se è una scena chiusa specifica l'entrata.

    bool IsLoaded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            print($"entered {gameObject.name}");

            light.color = lightColor;
            light.intensity = lightBrightness;

            Player.i.currentScene = this;

            if (gameObject.name != "Midgardr")
                NotificationsUI.i.AddNotification("entered " + gameObject.name);

            if (outdoor)
            {
                GameController.Instance.audioSource.clip = GameController.Instance.outdoorBackgroundTrack;
                GameController.Instance.audioSource.volume = .1f;
            }
            else
                GameController.Instance.audioSource.clip = GameController.Instance.indoorBackgroundTrack;

            GameController.Instance.audioSource.Play();
        }
        else if(collision.tag == "NPC")
        {
            if(collision.TryGetComponent(out NoAIBehaviour ai))
            {
                ai.triggeredScene = this;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadSceneAsMain()
    {
        LoadScene();
        foreach (var scene in connectedScenes)
            scene.LoadScene();

        //AstarPath.active.Scan();
    }

    public void LoadScene()
    {
        if(!IsLoaded)
        {
            //print(gameObject.name + " will be loaded");
            SceneManager.LoadScene(gameObject.name, LoadSceneMode.Additive);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //AstarPath.active.Scan();
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
