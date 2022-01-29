using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : MonoBehaviour, IEntity
{
    protected bool tamed = false;
    [SerializeField] int amountToEat; // this will be randomized with a nearly number
    int ate = 0;
    public int hp = 10;
    [SerializeField] protected Skill tameskill;
    [SerializeField] protected Sprite bloodSprite;
    [SerializeField] protected Curative meat;

    public virtual void Interact(Player player)
    {
        if (!tamed)
            nonTamedAction();
        else
            TamedAction();
    }

    public virtual void Eat(ItemBase food)
    {
        if(food is Curative)
        {
            ate++;
            if(ate >= amountToEat)
            {
                if (!tamed)
                    tamed = true;
                // else give hps
            }
            //Player.i.inventory.Remove(food);
        }
    }

    public void Plund()
    {
        
    }

    public void Transport()
    {
        Player.i.transportingAnimal = this;
        Player.i.animator.SetFloat("FacingHorizontal", -Player.i.animator.GetFloat("FacingHorizontal"));
        Player.i.animator.SetFloat("FacingVertical", -Player.i.animator.GetFloat("FacingVertical"));
        Player.i.animator.SetBool("carrying", true);

        GetComponent<BoxCollider2D>().enabled = false;
    }

    public abstract void nonTamedAction();
    public abstract void TamedAction();

    public void takeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        GetComponent<Animator>().SetBool("dead", true);
    }

    private void FixedUpdate()
    {
        if(Player.i.isInRange(this))
        {
            if (transform.childCount == 0)
                ShowSignal(); // tf y is not updating?
        }
        else
        {
            if(transform.childCount > 0)
                unShowSignal();
        }
    }

    public void unShowSignal()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowSignal()
    {
        if(!tamed) // tame signal 
        {
            if(Player.i.inventory.Skills.Contains(tameskill) || (Player.i.inventory.extraSlot != null && Player.i.inventory.extraSlot.item != null))
            {
                Instantiate(GameController.Instance.keytip_F, transform);
            }
        }
        else // ride signal
        {
            Instantiate(GameController.Instance.keytip_Z, transform);
        }
    }
}
