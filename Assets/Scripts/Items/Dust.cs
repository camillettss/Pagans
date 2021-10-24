using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Magic/Dust")]
public class Dust : ItemBase
{
    public void onHit()
    {
        Debug.Log("using dust");
    }

    public override void Use(Player player)
    {

    }
}
