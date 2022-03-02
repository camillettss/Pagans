using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Runner : MonoBehaviour, UIController
{
    [SerializeField] GameObject optionsContainer;

    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    [SerializeField] PlayerInput pinput;
    

    int sel = 0;

    public void onSubmit(InputAction.CallbackContext _)
    {
        Perform();
    }

    public void onCancel(InputAction.CallbackContext _)
    {

    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>().y;

        var prev = sel;

        if (input > 0) --sel;
        else if(input < 0) ++sel;

        sel = Mathf.Clamp(sel, 0, 2);

        if (prev != sel)
            UpdateSelection();
    }

    private void OnDisable()
    {
        pinput.actions["Submit"].performed -= onSubmit;
        pinput.actions["Navigate"].performed -= onNavigate;
        pinput.actions["Cancel"].performed -= onCancel;
    }

    private void OnEnable()
    {
        pinput.SwitchCurrentActionMap("UI");

        pinput.actions["Submit"].performed += onSubmit;
        pinput.actions["Navigate"].performed += onNavigate;
        pinput.actions["Cancel"].performed += onCancel;
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
            Play();
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

    void Play()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            // leggi i dati binari
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            PlayerData pd = bf.Deserialize(fs) as PlayerData;
            fs.Close();

            // verifica l'integrità del salvataggio
            try
            {
                if (pd.firstLaunch)
                {
                    SceneManager.LoadScene("Tutorial");
                }
                else
                {
                    SceneManager.LoadScene("Gameplay");
                }
            }
            catch
            {
                Debug.LogError("File di salvataggio corrotto, effettua un reset.");
                SceneManager.LoadScene("Tutorial");
            }
        }
        else // se non ci sono file di salvataggio allora è il primo avvio
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}
