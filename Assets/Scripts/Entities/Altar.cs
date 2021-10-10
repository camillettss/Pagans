using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject fireExplosion;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.activeAltar = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.activeAltar = null;
    }

    public IEnumerator Use()
    {
        Player.i.animator.SetFloat("FacingHorizontal", 0);
        Player.i.animator.SetFloat("FacingVertical", -1);
        Player.i.canMove = false;

        yield return StartCoroutine(fireAnim());
        ChooseWeapon();
        Player.i.canMove = true;
    }

    IEnumerator fireAnim()
    {
        fireExplosion.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(.8f);
    }

    public void ChooseWeapon()
    {
        // show a list, return quella su cui preme Z
        GameController.Instance.OpenState(GameState.ChoosingItem);

    }
}
