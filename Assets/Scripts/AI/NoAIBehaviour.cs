using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NoAIBehaviour : MonoBehaviour
{
    [SerializeField] int morninghour = 6;
    [SerializeField] int nighthour = 19;
    [SerializeField] List<Vector2> outdoorPositions;

    public CityDetails triggeredCity;
    public SceneDetails triggeredScene;

    AIPath aipath;
    Animator animator;

    float timer = 0;
    float actualTimeTarget = 5f;

    bool isGoingSomewhere = false;
    bool isGoingOutdoor = false;

    private void Awake()
    {
        aipath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        aipath.canMove = false;

        // be sure this gameobject has no gfx controller attached.
        /*if (TryGetComponent(out AIEntityGFX _))
            GetComponent<AIEntityGFX>().enabled = false;*/
    }

    public void AtHour(int hour)
    {
        if(hour == nighthour)
        {
            isGoingSomewhere = true;
            aipath.canMove = true;

            var target = triggeredCity.Houses[Random.Range(0, triggeredCity.Houses.Count)].position;
            print(target);

            aipath.destination = target; // choose random house
            isGoingOutdoor = false;
        }
        else if(hour == morninghour)
        {
            /*isGoingSomewhere = true;
            aipath.canMove = true;

            aipath.destination = triggeredCity.door.position;
            isGoingOutdoor = true;*/
        }
    }

    void noAIUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (aipath.canMove && aipath.reachedDestination)
            aipath.canMove = false;

        if (timer >= actualTimeTarget)
        {
            var dir = getRandomDirection();
            LookAt(dir);

            actualTimeTarget = Random.Range(5, 15);
            timer = 0f;
            if (Random.Range(0, 1) == 0) // 50%
            {

            }
            /*LookAt(getRandomDirection());
            actualTimeTarget = Random.Range(5, 15);
            timer = 0f;
            if (Random.Range(0, 1) == 0) // 50%
            {
                aipath.canMove = true;
                aipath.destination = new Vector3(transform.position.x + animator.GetFloat("FaceX"), transform.position.y + animator.GetFloat("FaceY"), transform.position.z);
            }
            else
                aipath.canMove = false;*/
        }
    }

    private void FixedUpdate()
    {
        if(!isGoingSomewhere)
            noAIUpdate();
        else
        {
            animator.SetFloat("speed", aipath.velocity.magnitude);

            if (aipath.desiredVelocity.x != 0)
            {
                animator.SetFloat("FaceX", aipath.desiredVelocity.x);
            }
            if (aipath.desiredVelocity.y != 0)
            {
                animator.SetFloat("FaceY", aipath.desiredVelocity.y);
            }

            if (aipath.reachedDestination)
            {
                print("reached destination in the update.");
                isGoingSomewhere = false;
                aipath.canMove = false;
                animator.SetFloat("speed", 0);
            }
        }
    }

    public void onEnterDoor(Portal door)
    {
        if(door.transform.position == aipath.destination)
        {
            if (!isGoingOutdoor)
            {
                aipath.canMove = false;
                aipath.destination = transform.position;
                isGoingSomewhere = false;
            }
            else
            {
                isGoingSomewhere = true;
                aipath.destination = outdoorPositions[0];
            }
        }
        animator.SetFloat("speed", 0);
    }

    IEnumerator waitForReach(System.Action onEndAction)
    {
        while (!aipath.reachedDestination)
            yield return new WaitForFixedUpdate();

        onEndAction?.Invoke();
    }

    void LookAt(Vector2 direction)
    {
        animator.SetFloat("FaceX", direction.x);
        animator.SetFloat("FaceY", direction.y);
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
}
