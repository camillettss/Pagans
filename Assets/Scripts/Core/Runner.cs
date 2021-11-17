using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Runner : MonoBehaviour
{
    [SerializeField] GameObject optionsContainer;

    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    int sel;

    private void Start()
    {
        sel = 0;
    }

    private void Update()
    {
        var prev = sel;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            sel++;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --sel;

        sel = Mathf.Clamp(sel, 0, 2); // sti cazzi che è hardcoded, fanculo le convenzioni lasciatemi morire da solo

        if (prev != sel)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            Perform();
    }

    void UpdateSelection()
    {
        for(int i=0; i<3; i++) // tutto hardcode vaffanculo stronzi
        {
            var t = optionsContainer.transform.GetChild(i).GetComponent<Text>();

            if (i == sel)
                t.color = selectedColor;
            else
                t.color = unselectedColor;
        }
    }

    void Perform()
    {
        if(sel == 0) // play
        {
            SceneManager.LoadScene("TutorialMain"); // loads gameplay, and it choose a scene.
        }
        else if(sel == 1) // Reset
        {
            SaveSystem.Reset();
        }
        else if(sel == 2)
        {
            Application.Quit();
        }
    }
}
