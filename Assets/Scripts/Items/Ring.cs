using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/new Ring")]
public class Ring : ItemBase
{
    [Header("Ring")]
    [SerializeField] int fortune;

    public override void Use(Player player)
    {
        Debug.Log($"{name}'s use() non ha ricevuto i parametri necessari quindi non è stato possibile chiamare l'overload.");
    }

    public override void Use(Player player, IEntity npc, int damage = 1)
    {
        Debug.Log("using a fcking ring");
    }
}
