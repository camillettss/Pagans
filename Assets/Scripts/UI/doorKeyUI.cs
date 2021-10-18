using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorKeyUI : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image keyImage;

    public void Open(Key key)
    {
        keyImage.sprite = key.icon;
        gameObject.SetActive(true);
    }
}
