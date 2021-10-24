using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath aiPath;
    public Animator animator;

    private void Update()
    {
        if (aiPath.desiredVelocity != Vector3.zero)
            animator.SetFloat("speed", 1f);
        else
            animator.SetFloat("speed", 0f);


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
