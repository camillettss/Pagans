using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using System;
using UnityEngine.InputSystem;

public class SettingsUI : MonoBehaviour, UIController
{
    int selected = 0;
    [SerializeField] Text diagonalMovesText;

    private void OnEnable()
    {
        Player.i.playerInput.SwitchCurrentActionMap("UI");
        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
    }

    private void OnDisable()
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
        Player.i.playerInput.actions["Navigate"].performed -= onNavigate;
        Player.i.playerInput.actions["Cancel"].performed -= onCancel;
    }

    private void Awake()
    {
        selected = 0;
        UpdateSelection();
        diagonalMovesText.text = $"diagonal:{Player.i.enableDiagonalMovements}";
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
            Player.i.Save(); // Save func

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

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        Perform(selected);
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        gameObject.SetActive(false);
        GameController.Instance.state = GameState.FreeRoam;
        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>().y;

        if (input > 0) --selected;
        else if (input < 0) ++selected;

        selected = Mathf.Clamp(selected, 0, transform.childCount - 1);
        UpdateSelection();
    }
}
