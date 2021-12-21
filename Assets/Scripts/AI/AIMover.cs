using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMover : MonoBehaviour
{
    NavMeshAgent agent;
    Animator m_anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 2.5f;
    }

    private void Update()
    {
        agent.SetDestination(getDestination());
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);

        m_anim.SetFloat("speed", agent.velocity.magnitude);
        m_anim.SetFloat("FaceX", Mathf.Clamp(agent.velocity.x, -1, 1));
        m_anim.SetFloat("FaceY", Mathf.Clamp(agent.velocity.y, -1, 1));
    }

    Vector3 getDestination()
    {
        if (GameController.Instance.hours == 8)
        {
            // ADD BEHAVIOURS
        }
        return transform.position; // ultima condizione: non muoverti.
    }

    
}
