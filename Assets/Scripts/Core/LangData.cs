using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LangData : MonoBehaviour
{
    public string lid => LocalizationSettings.SelectedLocale.Identifier.Code;
}
