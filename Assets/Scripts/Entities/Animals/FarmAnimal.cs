using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FarmAnimal : Animal
{
    bool hasProduct = false;
    bool waiting = false;
    int count = 0;

    bool inLove = false;

    Animator m_anim;
    AudioSource source;

    [SerializeField] ItemBase product = null;
    [SerializeField] Baby babySon;

    float timer=0;
    float maxTimer = 3f;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, Player.i.transform.position) <= 20 && !source.isPlaying && timer >= maxTimer)
        {
            source.Play();
            timer = 0f;
            maxTimer = Random.Range(3, 5);
        }
        else
            timer += Time.fixedDeltaTime;
    }

    public override void Interact(Player player)
    {
        if(product != null)
        {
            if (hasProduct)
            {
                player.inventory.Add(product); // TODO: secchi
                hasProduct = false;
                StartCoroutine(waitForMilk());
            }
            else
            {
                if (!waiting)
                {
                    StartCoroutine(waitForMilk());
                }
                else
                {
                    print($"count: {count}");
                }
            }
        }
    }

    public override void Eat(ItemBase food)
    {
        if (!inLove)
        {
            inLove = false;
            print("now is in love.");
            StartCoroutine(BecomePregnant());
        }
    }

    IEnumerator BecomePregnant()
    {
        if (cowInRange(out FarmAnimal cow))
        {
            while (Vector3.Distance(transform.position, cow.transform.position) > 1.7)
            {
                FollowTarget(cow.transform);
                yield return new WaitForFixedUpdate();
            }
            m_anim.SetFloat("speed", 0f);
            var son = Instantiate(babySon);
            son.Spawn(this);
        }
        else
            print("no near cow");
    }

    bool isInRange(IEntity entity, float radius = 1.5f)
    {
        foreach (var item in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            if (item.GetComponent<IEntity>() == entity)
                return true;
        }
        return false;
    }

    /*bool TinRange<T>(out T obj, float radius = 5f)
    {
        foreach (var item in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            if (item.TryGetComponent(out T res))
            {
                obj = res;
                return true;
            }
        }
        obj = default(T);
        print("returning False");
        return false;
    }*/

    bool cowInRange(out FarmAnimal cow, float radius = 5f)
    {
        foreach(var collider in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            if(collider.TryGetComponent<FarmAnimal>(out FarmAnimal e))
            {
                if(e != this)
                {
                    cow = e;
                    return true;
                }
            }
        }
        cow = default(FarmAnimal);
        return false;
    }

    IEnumerator waitForMilk()
    {
        waiting = true;
        for(int i = 0; i<11; i++)
        {
            yield return new WaitForSeconds(1f);
            count++;
        }
        waiting = false;
        hasProduct = true;
        count = 0;
    }

    void FollowTarget(Transform target)
    {
        print("following");
        m_anim.SetFloat("speed", 3.5f);
        m_anim.SetFloat("FaceX", (target.position.x - transform.position.x));
        m_anim.SetFloat("FaceY", (target.position.y - transform.position.y));

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3.5f * Time.fixedDeltaTime);
    }

    public override void nonTamedAction()
    {
        
    }

    public override void TamedAction()
    {
        
    }
}
