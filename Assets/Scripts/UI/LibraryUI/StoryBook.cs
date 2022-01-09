using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New story book")]
public class StoryBook : ItemBase
{
    public List<Page> Pages;

    public override void Use(Player player)
    {
        
    }

    public void AddPage(string data)
    {
        Pages.Add(new Page(data));
    }
}

[System.Serializable]
public class Page
{
    // <summary> max 100 characters. </summary>
    public string content;

    public Page(string data)
    {
        content = data;
    }
}