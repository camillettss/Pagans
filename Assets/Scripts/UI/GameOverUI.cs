using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Sprite X_selected;
    [SerializeField] Sprite X_unselected;

    [SerializeField] Sprite Y_selected;
    [SerializeField] Sprite Y_unselected;

    [SerializeField] Image XBtn;
    [SerializeField] Image YBtn;

    int selected = 0;

    private void Awake()
    {
        updateSelection();
    }

    public void HandleUpdate()
    {
        /*int prev = selected;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            selected = 0;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            selected = 1;

        if(selected != prev)
        {
            updateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
            Perform();*/
    }

    void updateSelection()
    {
        if(selected == 0)
        {
            XBtn.sprite = X_selected;
            YBtn.sprite = Y_unselected;
        }
        else if(selected == 1)
        {
            XBtn.sprite = X_unselected;
            YBtn.sprite = Y_selected;
        }
    }

    void Perform()
    {
        if(selected == 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Player.i.Load(); // loads the last save.
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }
    }
}
