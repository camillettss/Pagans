using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantDetails : MonoBehaviour
{
    [SerializeField] Text daysLeftText;
    [SerializeField] GameObject Ztip;

    private void Awake()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (Player.i.activePlant != null)
        {
            daysLeftText.text = $"{Player.i.activePlant.daysToGrow - Player.i.activePlant.daysPassed} giorni rimanenti,\nsemi di BAOBAB";
            Ztip.SetActive(true);
        }
        else
        {
            daysLeftText.text = "Vai sopra ad una pianta per selezionarla.";
            Ztip.SetActive(false);
        }
    }
}
