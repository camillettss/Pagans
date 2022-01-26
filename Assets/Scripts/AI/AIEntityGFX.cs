using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIEntityGFX : MonoBehaviour
{
    public AIPath aiPath;
    public Animator animator;

    private void FixedUpdate()
    {
        animator.SetFloat("speed", aiPath.velocity.sqrMagnitude);

        if (aiPath.remainingDistance <= .15f)
            animator.SetFloat("speed", 0);

        if(aiPath.desiredVelocity.x != 0)
        {
            animator.SetFloat("FaceX", aiPath.desiredVelocity.x);
        }
        if (aiPath.desiredVelocity.y != 0)
        {
            animator.SetFloat("FaceY", aiPath.desiredVelocity.y);
        }
    }
}
