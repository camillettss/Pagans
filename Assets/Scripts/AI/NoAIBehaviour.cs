using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAIBehaviour : MonoBehaviour
{
    [SerializeField] int morninghour = 8;
    [SerializeField] int nighthour = 19;

    public CityDetails triggeredCity;

    public void AtHour(int hour)
    {
        if(hour == nighthour)
        {

        }
        else if(hour == morninghour)
        {

        }
    }
}
