using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using System;

public class MenuController : MonoBehaviour, UIController
{
    int selected = 0;

    private void OnEnable()
    {
        selected = 0;
        UpdateSelection();

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

    void UpdateSelection()
    {
        for(int i=0; i<4; i++)
        {
            if(i==selected)
                transform.GetChild(i).GetComponent<UnityEngine.UI.Text>().color = GameController.Instance.selectedDefaultColor;
            else
                transform.GetChild(i).GetComponent<UnityEngine.UI.Text>().color = GameController.Instance.unselectedDefaultColor;
        }
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

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        Perform(selected);
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        gameObject.SetActive(false);
        GameController.Instance.state = GameController.Instance.prevState;
        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();

        if (input.y < 0)
            selected++;
        else if (input.y > 0)
            selected--;

        selected = Mathf.Clamp(selected, 0, 3);

        UpdateSelection();
    }
}
