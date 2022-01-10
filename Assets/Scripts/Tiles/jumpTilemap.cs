using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class jumpTilemap : MonoBehaviour
{
    bool playerIn;

    private void OnCollisionExit2D(Collision2D _)
    {
        playerIn = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            playerIn = true;
            StartCoroutine(StopAndWaitForLand());
        }
    }

    IEnumerator StopAndWaitForLand()
    {
        Player.i.animator.SetTrigger("jumpDown");
        yield return new WaitForSeconds(.3f);
        Player.i.transform.position = new Vector3(Player.i.transform.position.x, Player.i.transform.position.y - 1, Player.i.transform.position.z);
    }
}
