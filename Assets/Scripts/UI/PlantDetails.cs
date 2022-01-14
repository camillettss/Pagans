using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantDetails : MonoBehaviour
{
    [SerializeField] Text daysLeftText;

    private void Awake()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        daysLeftText.text = $"{Player.i.activePlant.daysToGrow - Player.i.activePlant.daysPassed} giorni rimanenti,\nsemi di BAOBAB";
    }
}
