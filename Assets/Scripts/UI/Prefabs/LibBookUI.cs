using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibBookUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;

    public void SetData(Book book)
    {
        icon.sprite = book.icon;
        nameText.text = book.Name;
    }
}
