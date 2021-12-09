using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class LibBookUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public StoryBook book;

    public void SetData(StoryBook book)
    {
        icon.sprite = book.icon;
        nameText.text = book.Name;
        this.book = book;
    }

    public void choose()
    {
        GetComponent<Animator>().SetTrigger("choose");
    }
}
