using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMover : MonoBehaviour
{
    [SerializeField] List<TimedLocation> behaviourDescriptor;

    NavMeshAgent agent;
    Animator m_anim;
    public CityDetails triggeredCity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 2.5f;
        agent.stoppingDistance = 0f;

        behaviourDescriptor = new List<TimedLocation>()
        {
            new TimedLocation(new Vector2(248, -35), 8)
        }; // definisco un comportamento statico.
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1); // set position restriction: z always 1.

        m_anim.SetFloat("speed", agent.velocity.magnitude);
        m_anim.SetFloat("FaceX", Mathf.Clamp(agent.velocity.x, -1, 1));
        m_anim.SetFloat("FaceY", Mathf.Clamp(agent.velocity.y, -1, 1));
    }

    Vector3 getDestination()
    {
        foreach(var loc in behaviourDescriptor)
        {
            if(loc.hour == GameController.Instance.hours)
            {
                return loc.destination;
            }
        }
        return transform.position; // ultima condizione: non muoverti.
    }

    public void GoHome()
    {
        // primary ai find home's door position
        // secondary ai (A*) reaches it
        //agent.SetDestination(triggeredCity.Houses[Random.Range(0, triggeredCity.Houses.Count)]); // va in una casa a caso.
        // if scene.indoor go to scene.exit, then city.house.
    }

    public void Reach(Vector2 destination)
    {
        agent.SetDestination(destination);
    }
    
}

[System.Serializable]
internal class TimedLocation
{
    public Vector2 destination;
    public int hour;

    public TimedLocation(Vector2 dest, int hour)
    {
        destination = dest;
        this.hour = hour;
    }
}
