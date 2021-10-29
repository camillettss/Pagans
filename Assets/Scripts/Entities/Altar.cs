using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject fireExplosion;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.i.activeAltar = this;
        animator.SetBool("isOn", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.i.activeAltar = null;
        animator.SetBool("isOn", false);
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
        GameController.Instance.ShowInfo("a causa del bug B#04 questa funzione è stata disabilità. sarà nuovamente disponibile nel nuovo aggiornamento (1.5+).", ()=> { }, 2.5f);
        //GameController.Instance.OpenState(GameState.ChoosingItem); // Remmed for B04. reactivate when fixed!

    }
}
