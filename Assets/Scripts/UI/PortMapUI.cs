using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortMapUI : MonoBehaviour, UIController
{
    public List<MapPoint> points;
    int selected = 0;

    [SerializeField] Sprite selectedPointSprite;
    [SerializeField] Sprite unselectedPointSprite;

    private void OnDisable()
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
        Player.i.playerInput.actions["Navigate"].performed -= onNavigate;
        Player.i.playerInput.actions["Cancel"].performed -= onCancel;

        try
        {
            Player.i.playerInput.SwitchCurrentActionMap("Player");
        }
        catch { } // capita se si chiude il gioco con la UI aperta.
    }

    private void OnEnable()
    {
        Player.i.playerInput.SwitchCurrentActionMap("UI");

        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        gameObject.SetActive(false);
        GameController.Instance.state = GameState.FreeRoam; // lo switch lo chiamo in onDisable
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();

        if (input.y < 0 || input.x > 0) --selected;
        else if (input.y > 0 || input.x < 0) ++selected;

        selected = Mathf.Clamp(selected, 0, points.Count - 1);

        UpdateSelection();
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        var selectedPoint = points[selected];
        print(selectedPoint.target_chunkName);
    }

    public void UpdateSelection()
    {
        for(int i=0; i<points.Count; i++)
        {
            if (i == selected)
                points[i].image.sprite = selectedPointSprite;
            else
                points[i].image.sprite = unselectedPointSprite;
        }
    }


}
