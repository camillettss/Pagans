using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{
    [SerializeField] int speed = 5;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] int damage = 50;
    [SerializeField] float attackRate = 2f;
    float nextAttackTime = 0f;

    Animator animator;
    Rigidbody2D rb;

    float moveX = 0.0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveX = Input.GetAxis("Horizontal");

        if (moveX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (moveX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (Mathf.Abs(moveX) > Mathf.Epsilon)
            animator.SetInteger("AnimState", 1);
        else
            animator.SetInteger("AnimState", 0);

        if(Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

    }

    void Attack()
    {
        // play anim
        animator.SetTrigger("Attack");

        // detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<BattleEnemy>().TakeDamage(damage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
