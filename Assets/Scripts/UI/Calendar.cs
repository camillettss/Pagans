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

    [SerializeField] List<Month> months;

    int selected_month = 0;
    int selected_day = 0;

    public Month actualMonth;
    public Day today;

    public List<Month> Months => months;

    List<CalendarDayUI> slotUIs;

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if (!dayDetails.isActiveAndEnabled)
            {
                gameObject.SetActive(false);
                GameController.Instance.state = GameState.FreeRoam;
            }
            else
            {
                dayDetails.gameObject.SetActive(false);
                daysContainer.gameObject.SetActive(true);
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
        }
    }

    void ViewDetails()
    {
        dayDetails.gameObject.SetActive(true);
        daysContainer.gameObject.SetActive(false);
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
            if(months[selected_month] == actualMonth)
            {
                if (i == selected_day)
                {
                    if(today.dayNo == i+1)
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
                    if(today.dayNo == i + 1)
                    {
                        slotUIs[i].Border.color = GameController.Instance.unselectedActualDayColor;
                    }
                    else
                    {
                        slotUIs[i].Border.color = GameController.Instance.unselectedDayDefaultColor;
                    }
                }
            }
            else
            {
                slotUIs[i].Border.color = GameController.Instance.unselectedDefaultColor;
            }
        }

        if (selected_day == -1)
            Title.color = GameController.Instance.selectedDefaultColor; // TODO: show arrows
        else
            Title.color = Color.black;
    }

    List<Day> daysOfMonth(int monthNo)
    {
        return months[monthNo].days;
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

    public bool isFest = false;
    public Sprite festivitySprite;
}