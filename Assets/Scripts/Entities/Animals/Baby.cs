using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baby : MonoBehaviour
{
    // every baby has the same behaviour: follow their mom
    Animal Mom;
    bool goingToMom = false;
    Animator m_anim;

    public void Spawn(Animal mom)
    {
        Mom = mom;
        m_anim = GetComponent<Animator>();

        transform.position = mom.transform.position;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Mom.transform.position) > 5 && !goingToMom)
        {
            StartCoroutine(FollowMom());
        }
    }

    IEnumerator FollowMom()
    {
        goingToMom = true;
        while(Vector3.Distance(transform.position, Mom.transform.position) > 2)
        {
            m_anim.SetFloat("speed", 3.5f);
            m_anim.SetFloat("FaceX", (Mom.transform.position.x - transform.position.x));
            m_anim.SetFloat("FaceY", (Mom.transform.position.y - transform.position.y));

            transform.position = Vector3.MoveTowards(transform.position, Mom.transform.position, 3.5f * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
        goingToMom = false;
        m_anim.SetFloat("speed", 0f);
    }
}
