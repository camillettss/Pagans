using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using System;

public class SettingsUI : MonoBehaviour
{
    int selected = 0;
    [SerializeField] Text diagonalMovesText;

    private void Awake()
    {
        selected = 0;
        UpdateSelection();
        diagonalMovesText.text = $"diagonal:{Player.i.enableDiagonalMovements}";
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

        selected = Mathf.Clamp(selected, 0, transform.childCount - 1);

        if (prev != selected)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            Perform(selected);
    }

    public void UpdateSelection()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            if(transform.GetChild(i).TryGetComponent(out Text text))
            {
                if (i == selected)
                    text.color = GameController.Instance.selectedDefaultColor;
                else
                    text.color = GameController.Instance.unselectedDefaultColor;
            }
        }
    }

    void Perform(int choosen)
    {
        if (choosen == 0) // Save
        {
            Player.i.Save(); // Save

            // Back to freeroam;
            gameObject.SetActive(false);
            GameController.Instance.menu.gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }
        else if (choosen == 1) // Load
        {
            Player.i.Load();
        }

        else if(choosen == 2) // toggle diagonal movements
        {
            Player.i.enableDiagonalMovements = !Player.i.enableDiagonalMovements;
            diagonalMovesText.text = $"diagonal:{Player.i.enableDiagonalMovements}";
        }

        else if (choosen == 3) // change language
        {
            for (int i = 0; i < transform.childCount; i++) // deselect all
            {
                if (transform.GetChild(i).TryGetComponent(out Text text))
                {
                    text.color = GameController.Instance.unselectedDefaultColor;
                }
            }

            GameController.Instance.languageContentController.Activate();
        }
    }
}
