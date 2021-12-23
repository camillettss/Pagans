using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

// this file will replace AI Destination Setter
[RequireComponent(typeof(NPCController))]
[RequireComponent(typeof(AIPath))]
public class PrimaryAI : MonoBehaviour
{
    public Transform target;

    public List<Vector2> targets;
    int actTarget = 0;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    Animator animator;
    NPCController attachedNPC;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attachedNPC = GetComponent<NPCController>();

        InvokeRepeating("CheckTargets", 0f, 1f);
    }

    public void CheckTargets()
    {
        seeker.StartPath(rb.position, getNewTarget(), OnPathComplete);
    }

    [System.Obsolete("funzione non ancora definita.")]
    Vector2 getNewTarget()
    {
        return Vector2.zero;
    }

    void OnPathComplete(Path p)
    {
        print("Path done");
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

}
