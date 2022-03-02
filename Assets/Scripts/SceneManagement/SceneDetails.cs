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

            LoadSceneAsMain();
            Player.i.SetScene(this);

            // unload chunks that are no longer connected
            if (Player.i.prevScene != null)
            {
                var prevLoadedScenes = Player.i.prevScene.connectedScenes;
                foreach (var scene in prevLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        print($"unloading {scene}");
                        scene.UnloadScene();
                    }
                }
            }

            light.color = lightColor;
            light.intensity = lightBrightness;

            /*if (gameObject.name != "Midgardr")
                NotificationsUI.i.AddNotification("entered " + gameObject.name);*/

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

    public void LoadSceneAsMain()
    {
        LoadScene();
        foreach (var scene in connectedScenes)
            scene.LoadScene();
    }

    public void LoadScene()
    {
        print($"loading {gameObject.name}");
        if(!IsLoaded)
        {
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
