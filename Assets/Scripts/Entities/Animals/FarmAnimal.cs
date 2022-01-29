using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Pathfinding;

public class FarmAnimal : Animal
{
    bool hasProduct = false;
    bool waiting = false;
    bool meatTaken = false;

    public bool isMale = false;

    int count = 0;
    int birthHour;

    bool inLove = false;

    Animator m_anim;
    AudioSource source;

    [SerializeField] ItemBase product = null;
    [SerializeField] FarmAnimal animalPrefab;

    float timer=0;
    float maxTimer = 3f;

    CustomAIPath ai;

    [Header("babies options")]
    [SerializeField] int daysToGrow = 0;

    // setted by FarmAnimal:Spawn();
    int daysPassed = 0;
    FarmAnimal mom;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        ai = GetComponent<CustomAIPath>();
    }

    private void FixedUpdate()
    {
        if (timer >= maxTimer && !source.isPlaying && Vector2.Distance(transform.position, Player.i.transform.position) <= 20)
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
        if(hp <= 0)
        {
            if (!meatTaken)
            {
                GetComponent<Animator>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = bloodSprite;
                player.inventory.Add(meat);
                meatTaken = true;
            }
        }
        else
        {
            if (product != null)
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
    }

    public void Spawn(FarmAnimal mom)
    {
        transform.position = mom.transform.position;
        this.mom = mom;
        daysToGrow = 2;
        daysPassed = 0;
        m_anim.SetBool("grown", false);
    }

    public void newDay()
    {
        if (daysPassed < daysToGrow)
            daysPassed++;
        else
            return;

        if(daysPassed >= daysToGrow && !m_anim.GetBool("grown"))
        {
            // crescono tutti a mezzanotte, add che scelgono un orario casuale.
            Grow(); // come crescono in fretta :')
        }
    }

    void Grow()
    {
        // change animator controller
        m_anim.SetBool("grown", true);
    }

    public override void Eat(ItemBase food)
    {
        if (isMale)
        {
            if (m_anim.GetBool("grown") && !inLove)
                MakeBaby();
            else
                hp += ((Curative)food).cure;
        }
        else
            hp += ((Curative)food).cure;
    }

    void MakeBaby()
    {
        ai.alreadyCalledTargetReached = false;
        if (cowInRange(out FarmAnimal cow))
        {
            ai.destination = cow.transform.position;
        }
        else
            print("no near cow");
    }

    public void OnFemaleReached()
    {
        var son = Instantiate(animalPrefab);
        son.Spawn(this);
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
                if((e != this) && !(e.hp<=0))
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
