using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class LibBookUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public string story;

    public void SetData(StoryBook book)
    {
        icon.sprite = book.icon;
        nameText.text = book.Name;
        story = book.story;
    }

    public void choose()
    {
        GetComponent<Animator>().SetTrigger("choose");
    }
}
