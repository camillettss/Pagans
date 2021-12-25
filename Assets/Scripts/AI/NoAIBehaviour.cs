using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NoAIBehaviour : MonoBehaviour
{
    [SerializeField] int morninghour = 8;
    [SerializeField] int nighthour = 19;

    public CityDetails triggeredCity;
    public SceneDetails triggeredScene;

    AIPath aipath;
    Animator animator;

    float timer = 0;
    float actualTimeTarget = 5f;

    private void Awake()
    {
        aipath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        aipath.canMove = false;

        // be sure this gameobject has no gfx controller attached.
        if (TryGetComponent(out AIEntityGFX gfx))
            GetComponent<AIEntityGFX>().enabled = false;
    }

    public void AtHour(int hour)
    {
        if(hour == nighthour)
        {
            //goHome();
        }
        else if(hour == morninghour)
        {
            //StartCoroutine(exitHome());
        }
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(timer >= actualTimeTarget)
        {
            LookAt(getRandomDirection());
            actualTimeTarget = Random.Range(5, 15);
            timer = 0f;
        }
    }

    void LookAt(Vector2 direction)
    {
        animator.SetFloat("FaceX", direction.x);
        animator.SetFloat("FaceY", direction.y);

        print($"now looking at: {direction.x}, {direction.y}");
    }

    Vector2 getRandomDirection()
    {
        Vector2 res = Vector2.zero;

        res.x = new List<int>() { -1, 0, 1 }[Random.Range(0, 2)];

        if (res.x == 0)
            res.y = new List<int>() { -1, 1 }[Random.Range(0, 2)];
        else
            res.y = 0;

        return res;
    }

    void goHome()
    {
        
    }

    IEnumerator exitHome()
    {
        if (!triggeredScene.outdoor)
        {
            aipath.canMove = true;
            aipath.destination = triggeredCity.door.position; // metto un oggetto city dentro alla casa
            while (!aipath.reachedDestination)
            {
                yield return new WaitForFixedUpdate();
            }
            aipath.canMove = false;
        }
    }
}
