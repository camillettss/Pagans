using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableTilemap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.animator.SetBool("isClimbing", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.animator.SetBool("isClimbing", false);
    }
}
