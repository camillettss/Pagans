using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    sleeping,
    awake
}

public class EnemyController : MonoBehaviour
{
    float speed = 3.5f;
    Animator m_anim;
    Transform target;
    float awakeRange = 5f;
    float attackRange = 3f;

    EnemyState state;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        target = Player.i.transform;
    }

    private void Update()
    {
        if (state == EnemyState.sleeping)
        {
            if (Vector3.Distance(target.position, transform.position) <= awakeRange) // awake
                state = EnemyState.awake;
        }
        else if(state == EnemyState.awake)
        {
            if (Vector3.Distance(target.position, transform.position) <= attackRange)
            {
                StartCoroutine(Attack());
            }
            else
                FollowTarget();
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
        print("attacking");
        yield return new WaitForSeconds(.5f);
    }
}
