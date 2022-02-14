using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField] Text Title;
    [SerializeField] Transform daysContainer;
    [SerializeField] CalendarDayUI prefab;
    [SerializeField] Text dayDetails;
    [SerializeField] Text notes;
    [SerializeField] Transform notesContainer;

    [SerializeField] List<Month> months;

    int selected_month = 0;
    int selected_day = 0;

    public Month actualMonth;
    public Day today;

    public List<Month> Months => months;

    List<CalendarDayUI> slotUIs;

    private void Awake()
    {
        selected_month = months.IndexOf(actualMonth);
    }

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            if (!dayDetails.isActiveAndEnabled)
            {
                gameObject.SetActive(false);
                GameController.Instance.state = GameState.Inventory;
            }
            else
            {
                dayDetails.gameObject.SetActive(false);
                daysContainer.gameObject.SetActive(true);
                notesContainer.gameObject.SetActive(true);
            }
        }

        var prevday = selected_day;
        var prevmon = selected_month;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if (selected_day == -1)
                --selected_month;
            else
                --selected_day;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(selected_day == -1)
                ++selected_month;
            else
                ++selected_day;

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selected_day >= 0 && selected_day < 7)
            {
                selected_day = -1;
            }
            else
            {
                selected_day -= 7;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
            selected_day += 7;

        selected_day = Mathf.Clamp(selected_day, -1, slotUIs.Count - 1);
        selected_month = Mathf.Clamp(selected_month, 0, months.Count - 1);

        if (prevmon != selected_month)
            UpdateContents();

        if (prevday != selected_day)
            UpdateSelection();

        if(Input.GetKeyDown(KeyCode.Z))
        {
            ViewDetails();
        }*/
    }

    void ViewDetails()
    {
        dayDetails.gameObject.SetActive(true);
        daysContainer.gameObject.SetActive(false);
        notesContainer.gameObject.SetActive(false);
        if (selected_day != -1)
            dayDetails.text = daysOfMonth(selected_month)[selected_day].dayDescription;
        else
            dayDetails.text = months[selected_month].description;
    }

    public void UpdateContents()
    {
        foreach (Transform child in daysContainer)
            Destroy(child.gameObject);

        slotUIs = new List<CalendarDayUI>();

        foreach(var day in daysOfMonth(selected_month))
        {
            var slotUI = Instantiate(prefab, daysContainer);
            if (day == today)
                day.dayNote = "oggi.";
            slotUI.SetData(day);

            slotUIs.Add(slotUI);
        }

        Title.text = months[selected_month].name;

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected_day)
            {
                if (today.dayNo == i + 1 && actualMonth == months[selected_month])
                {
                    slotUIs[i].Border.color = GameController.Instance.selectedActualDayColor;
                }
                else
                {
                    slotUIs[i].Border.color = GameController.Instance.selectedDayDefaultColor;
                }
            }
            else
            {
                if (today.dayNo == i + 1 && actualMonth == months[selected_month])
                {
                    slotUIs[i].Border.color = GameController.Instance.unselectedActualDayColor;
                }
                else
                {
                    slotUIs[i].Border.color = GameController.Instance.unselectedDayDefaultColor;
                }
            }
        }

        if (selected_day == -1)
            Title.color = GameController.Instance.selectedDefaultColor; // TODO: show arrows
        else
        {
            Title.color = Color.black;
            notes.text = slotUIs[selected_day].note;
        }
    }

    List<Day> daysOfMonth(int monthNo)
    {
        return months[monthNo].days;
    }

    public void newDay()
    {
        if (today.dayNo < actualMonth.days.Count)
        {
            today = actualMonth.days[actualMonth.days.IndexOf(today) + 1];
        }
        else
        {
            actualMonth = months[months.IndexOf(actualMonth) + 1];
            today = actualMonth.days[0];
        }
    }
    
    public Date GetDate(int distance_from_today=0)
    {
        var date = new Date(today.dayNo, actualMonth);

        for(int i=0; i<distance_from_today; i++)
        {
            date.nextDay();
        }

        return date;
    }
}

[System.Serializable]
public class Date
{
    public int hour;
    public int day;
    public Month month;

    public Date(int day, Month month)
    {
        this.day = day;
        this.month = month;
    }

    public void nextDay()
    {
        day++;
        var months = GameController.Instance.calendar.Months;
        if (day > months[months.IndexOf(month)].days.Count)
        {
            day = 1;
            month = months[months.IndexOf(month) + 1];
        }
    }
}

[System.Serializable]
public class Month
{
    public string name;
    public string description;
    public List<Day> days = new List<Day>(30);
}

[System.Serializable]
public class Day
{
    public string name;
    public int dayNo;
    public string dayDescription = "un normalissimo giorno.";
    public string dayNote = "questo non è un giorno particolare, seleziona una festività e leggi qui le sue note.";

    public bool isFest = false;
    public Sprite festivitySprite;
}