using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.Tilemaps.TilemapCollider2D))]
public class Agrimap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player.i.activeAgrimap = this;
            Player.i.drawSelected_Agrimap = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player.i.activeAgrimap = null;
            Player.i.drawSelected_Agrimap = false;
            FindObjectOfType<GridController>().FlooshHoverTiles();
        }
    }
}
