using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayUI : MonoBehaviour
{
    public Text dayNumber;
    public Image festivityIcon;
    public Image Border;
    public string note;

    public void SetData(Day day)
    {
        dayNumber.text = $"{day.dayNo}";
        note = day.dayNote;
        if (day.isFest)
            festivityIcon.sprite = day.festivitySprite;
        else
            festivityIcon.enabled = false;
    }
}
