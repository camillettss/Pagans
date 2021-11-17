using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class StoryPlayer : MonoBehaviour
{
    // questo script controlla il player nelle scene bloccate.
    [SerializeField] List<Vector2> Movements;
    Animator animator;
    Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Move(0,1);
        }
    }

    void Move(Vector2 mvc) // mvc is like moveInput in Player.cs
    {
        animator.SetFloat("Horizontal", mvc.x);
        animator.SetFloat("Vertical", mvc.y);
        animator.SetFloat("Speed", mvc.sqrMagnitude);

        rb.velocity = mvc * 5f;
        rb.MovePosition(rb.position + mvc * 5f * Time.deltaTime);
    }

    void Move(int x, int y)
    {
        // (0, v) significa che devi avere newy = oldy+v
        // scrivere (x, y) in cui sia x che y != 0 è insensato poichè elabora solo una coordinata
        var targetPos = new Vector2(transform.position.x + x, transform.position.y+y);
        if(targetPos.x != transform.position.x) // è un punto con la stessa y
        {
            while(targetPos.x != transform.position.x) // mi muovo finchè non aumento precisamente di una x
            {
                Move(new Vector2(x,0));
            }
        }
        else if(targetPos.y != transform.position.y)
        {
            while(targetPos.y != transform.position.y)
            {
                Move(new Vector2(0, y));
            }
        }
    }
}
