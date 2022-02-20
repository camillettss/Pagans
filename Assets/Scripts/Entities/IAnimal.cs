using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class IAnimal : MonoBehaviour, IEntity
{
    [SerializeField] int hp = 15;
    protected bool tamed = false;
    Animator m_anim;

    private void Awake()
    {
        StartCoroutine(m_update());
        m_anim = GetComponent<Animator>();
    }

    IEnumerator m_update()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));

        m_anim.SetFloat("FaceX", choose(new List<int> { 1, -1 }));
        m_anim.SetFloat("FaceY", choose(new List<int> { 1, -1 }));
    }

    int choose(List<int> possibilities)
    {
        return possibilities[Random.Range(0, possibilities.Count)];
    }

    public virtual void Interact(Player player)
    {
        if (tamed)
            tamedInteraction();
        else
            nonTamedInteraction();
    }



    protected abstract void tamedInteraction();
    protected abstract void nonTamedInteraction();

    public void Eat(Curative food)
    {
        tamed = true; // too easy but perfect for now.
    }


    public void ShowSignal()
    {
        
    }

    public void takeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
