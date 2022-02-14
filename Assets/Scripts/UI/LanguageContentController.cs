using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageContentController : MonoBehaviour
{
    public Transform content;
    [SerializeField] TextUI TextPrefab;

    int selected = 0;
    List<TextUI> textUIs = new List<TextUI>();

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(UpdateContents());
        GameController.Instance.OpenState(GameState.ChooseLanguage);
    }

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
            GameController.Instance.OpenState(GameState.Settings); // gobject should be already activated
        }

        var prev = selected;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        selected = Mathf.Clamp(selected, 0, textUIs.Count - 1);

        if (prev != selected)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selected];
        }*/

    }

    IEnumerator UpdateContents()
    {
        yield return LocalizationSettings.InitializationOperation;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        textUIs = new List<TextUI>();

        foreach(var lang in LocalizationSettings.AvailableLocales.Locales)
        {
            print(lang);
            var textPrefab = Instantiate(TextPrefab, content);
            textPrefab.text.text = lang.name;

            textUIs.Add(textPrefab);
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i=0; i<textUIs.Count; i++)
        {
            if (i == selected)
            {
                textUIs[i].text.color = GameController.Instance.selectedDefaultColor;
            }
            else
                textUIs[i].text.color = GameController.Instance.unselectedDefaultColor;
        }
    }
}
