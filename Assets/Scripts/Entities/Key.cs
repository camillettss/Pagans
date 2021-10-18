using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/new key")]
public class Key : ItemBase
{
    public int keycode;
    public override void Use(Player player)
    {
        // usato SOLO dalla door.
        Debug.Log("using the key");
    }
}
