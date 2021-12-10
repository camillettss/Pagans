using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum EnemyState
{
    sleeping,
    awake
}

public enum EnemyType
{
    Archer,
    Swordman
}

public class EnemyController : MonoBehaviour, IEntity
{
    float speed = 3.5f;
    Animator m_anim;
    Transform target;
    float awakeRange = 5f;
    float attackRange = 1f;
    bool canAttack = true;
    int hp = 10;

    EnemyState state = EnemyState.sleeping;
    [SerializeField] EnemyType type = EnemyType.Swordman;
    [SerializeField] LayerMask PlayerLayer;
    [SerializeField] Transform AtkPoint;
    [SerializeField] int damage = 2;
    [SerializeField] int expDrop = 10;

    public string Name;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        target = Player.i.transform;
    }

    public void HandleUpdate() // this should become HandleUpdate and being called only if gobj is in fov
    {
        if (state == EnemyState.sleeping)
        {
            if (m_anim.GetFloat("speed") > 0)
                m_anim.SetFloat("speed", 0);

            if (Vector3.Distance(target.position, transform.position) <= awakeRange) // awake
                state = EnemyState.awake;
        }
        else if(state == EnemyState.awake)
        {
            if (Vector3.Distance(target.position, transform.position) <= attackRange)
            {
                AtkPoint.position = new Vector3(transform.position.x + m_anim.GetFloat("FacingHorizontal"), transform.position.y + m_anim.GetFloat("FacingVertical"), transform.position.z);

                if (m_anim.GetFloat("speed") > 0)
                    m_anim.SetFloat("speed", 0);

                if(canAttack)
                    StartCoroutine(Attack());
            }
            else
            {
                if (Vector3.Distance(target.position, transform.position) <= awakeRange)
                    FollowTarget();
                else
                    state = EnemyState.sleeping;
            }

        }
    }

    void FollowTarget()
    {
        m_anim.SetFloat("speed", speed);
        m_anim.SetFloat("FaceX", (target.position.x - transform.position.x));
        m_anim.SetFloat("FaceY", (target.position.y - transform.position.y));

        transform.position = Vector3.MoveTowards(transform.position, Player.i.transform.position, speed * Time.deltaTime);
    }

    IEnumerator Attack()
    {
        canAttack = false;
        if(type == EnemyType.Archer)
        {
            Shoot();
            yield return new WaitForSeconds(1f);
        }
        else if(type == EnemyType.Swordman)
        {
            yield return StartCoroutine(SwordAttack());
            yield return new WaitForSeconds(1f); // anti spam
        }
        canAttack = true;
    }

    void Shoot()
    {
        // gli arceri verranno aggiunti nella 1.5.1
    }

    IEnumerator SwordAttack()
    {
        m_anim.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AtkPoint.position, attackRange, PlayerLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Player>().GetDamage(damage);
            print($"player hitted, now life is:{Player.i.hp}");
        }

        yield return new WaitForSeconds(1f); // fine animazione
    }

    public void Interact(Player player)
    {
    }

    public void takeDamage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            if (Player.i.quest != null && Player.i.quest.goal != null)
            {
                Player.i.quest.goal[0].EnemyKilled(this);
            }
            Player.i.experience += expDrop;
            Destroy(gameObject);
        }
    }

    public void ShowSignal()
    {
    }
}
