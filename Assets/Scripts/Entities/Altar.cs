using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject fireExplosion;

    Animator animator;

    Animal sacrifice;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (Player.i.transportingAnimal != null)
                sacrifice = Player.i.transportingAnimal;

            Player.i.activeAltar = this;

            if(sacrifice != null)
                animator.SetBool("isOn", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (Player.i.transportingAnimal != null)
                sacrifice = null;

            Player.i.activeAltar = null;
            animator.SetBool("isOn", false);
        }
    }

    public IEnumerator Use()
    {
        if(sacrifice == null)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
            if(colliders.Length > 0)
            {
                foreach(var collider in colliders)
                {
                    if(collider.TryGetComponent(out Animal entity))
                    {
                        sacrifice = entity;
                        break;
                    }
                }
            }
        }
        if(sacrifice != null)
        {
            Player.i.animator.SetFloat("FacingHorizontal", 0);
            Player.i.animator.SetFloat("FacingVertical", -1);
            Player.i.canMove = false;

            yield return StartCoroutine(fireAnim());
            sacrifice.GetComponent<SpriteRenderer>().DOFade(0f, .5f);
            yield return new WaitForSeconds(.35f);
            Player.i.canMove = true;
            yield return new WaitForSeconds(.15f);
            Destroy(sacrifice.gameObject);
        }
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
