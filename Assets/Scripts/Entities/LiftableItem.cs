using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableItem : MonoBehaviour, IEntity
{
    bool lifted = false;
    [SerializeField] Sprite brokenSprite;

    public void Interact(Player player)
    {
        if (!lifted)
            StartCoroutine(Lift());
    }

    IEnumerator Lift()
    {
        Player.i.animator.SetBool("Lift", true);
        yield return new WaitForSeconds(0.5f);

        Player.i.liftingItem = this;
        lifted = true;

        GetComponent<BoxCollider2D>().enabled = false;
    }

    public IEnumerator Throw(bool inverse=false)
    {
        Player.i.liftingItem = null;
        lifted = false;

        float count = 0, timer = 0.5f;
        var lockpos = new Vector3();
        if (inverse)
            lockpos = new Vector3(Player.i.transform.position.x - Player.i.animator.GetFloat("FacingHorizontal"), Player.i.transform.position.y - Player.i.animator.GetFloat("FacingVertical"));
        else
            lockpos = new Vector3(Player.i.transform.position.x + Player.i.animator.GetFloat("FacingHorizontal"), Player.i.transform.position.y + Player.i.animator.GetFloat("FacingVertical"));
        while (count < timer)
        {
            transform.position = Vector3.MoveTowards(transform.position, lockpos, 5.5f * Time.deltaTime);
            count += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        takeDamage(1);
        Player.i.animator.SetBool("Lift", false);
    }

    public void ShowSignal()
    {
    }

    public void takeDamage(int dmg)
    {
        Break();
    }

    void Break()
    {
        if (brokenSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = brokenSprite;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
