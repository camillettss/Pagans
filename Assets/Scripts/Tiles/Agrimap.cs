using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.Tilemaps.TilemapCollider2D))]
public class Agrimap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.activeAgrimap = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.activeAgrimap = null;
    }
}
