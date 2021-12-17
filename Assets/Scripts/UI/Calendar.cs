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

    List<CalendarDayUI> slotUIs;

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if (!dayDetails.enabled)
            {
                gameObject.SetActive(false);
                GameController.Instance.state = GameState.FreeRoam;
            }
            else
            {
                dayDetails.enabled = false;
                daysContainer.gameObject.SetActive(true);
            }
        }

        var prevday = selected_day;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            selected_day--;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            selected_day++;

        selected_day = Mathf.Clamp(selected_day, 0, slotUIs.Count - 1);

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
        dayDetails.text = daysOfMonth(selected_month)[selected_day].dayDescription;
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

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for(int i=0; i<slotUIs.Count; i++)
        {
            if (i == selected_day)
                slotUIs[i].Border.color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].Border.color = Color.black;
        }
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
    public List<Day> days;
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