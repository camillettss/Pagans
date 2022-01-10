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
        if (collision.collider.tag == "Player" && Player.i.canJump)
        {
            playerIn = true;
            StartCoroutine(StopAndWaitForLand());
        }
    }

    IEnumerator StopAndWaitForLand()
    {
        Player.i.animator.SetTrigger("jumpDown");
        Player.i.canMove = false;

        var rb = Player.i.GetComponent<Rigidbody2D>();
        var moveInput = new Vector2(0, -1);

        while (playerIn)
        {
            rb.velocity = moveInput * 5;
            rb.MovePosition(rb.position + moveInput * 2 * Time.fixedDeltaTime);
            yield return new WaitForEndOfFrame();
        }

        Player.i.canMove = true;

    }
}
