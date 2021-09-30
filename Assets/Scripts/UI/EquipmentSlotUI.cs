using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] GameObject indicator;

    public void SetIndicator(bool val)
    {
        indicator.gameObject.SetActive(val);
    }
}
