using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuController : MonoBehaviour
{
    int selected = 0;

    private void Awake()
    {
        selected = 0;
        UpdateSelection();
    }

    public void HandleUpdate(Action onBack)
    {
        int prev = selected;
        if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;
        selected = Mathf.Clamp(selected, 0, transform.childCount-1);

        if (prev != selected)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            Perform(selected);
    }

    void UpdateSelection()
    {
        foreach(Transform child in transform)
            child.GetComponent<UnityEngine.UI.Text>().color = Color.black;
        
        transform.GetChild(selected).GetComponent<UnityEngine.UI.Text>().color = Color.cyan;
    }

    void Perform(int choosen)
    {
        if (choosen == 0) // Codex
        {
            gameObject.SetActive(false);
            GameController.Instance.OpenState(GameState.Quests);
        }
        else if (choosen == 1) // Bag
        {
            gameObject.SetActive(false);
            GameController.Instance.OpenState(GameState.Inventory);
        }

        else if (choosen == 2) // Settings
        {
            gameObject.SetActive(false);
            GameController.Instance.OpenState(GameState.Settings);
        }

        else if (choosen == 3) // Exit
            Application.Quit();
    }
}
