using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatformTilemap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.canJump = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.canJump = false;
    }
}
