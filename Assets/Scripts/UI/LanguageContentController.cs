using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageContentController : MonoBehaviour
{
    public Transform content;
    [SerializeField] TextUI TextPrefab;

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(UpdateContents());
    }

    IEnumerator UpdateContents()
    {
        yield return LocalizationSettings.InitializationOperation;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach(var lang in LocalizationSettings.AvailableLocales.Locales)
        {
            var textPrefab = Instantiate(TextPrefab, content);
            textPrefab.text.text = lang.name;
        }
    }
}
