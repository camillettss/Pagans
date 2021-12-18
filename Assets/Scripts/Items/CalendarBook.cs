using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "new calendar")]
public class CalendarBook : Book
{

    public override void Use(Player player)
    {
        GameController.Instance.OpenState(GameState.Calendar);
    }
}