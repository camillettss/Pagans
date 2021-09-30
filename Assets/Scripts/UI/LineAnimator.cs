using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField] float speed;

    private void Start()
    {
        line.widthMultiplier = 0.2f;
        line.positionCount = 2;
    }

    public void Reach(Vector3 target)
    {
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
