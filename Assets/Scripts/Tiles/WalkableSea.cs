using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class WalkableSea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.animator.SetBool("onLowSea", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.animator.SetBool("onLowSea", false);
    }
}
