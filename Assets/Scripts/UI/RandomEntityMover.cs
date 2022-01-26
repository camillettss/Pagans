using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomEntityMover : MonoBehaviour
{
    public float speed = 3f;
    public float tolerance = 40f;
    Tree target;

    List<Tree> lastTwoTrees = new List<Tree>(2);

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = getNearestTree();
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.fixedDeltaTime);
        if(Vector2.Distance(transform.position, target.transform.position) <= 0.5f)
        {
            lastTwoTrees.Add(target);
            if (lastTwoTrees.Count > 2)
                lastTwoTrees.RemoveAt(0);

            target = getNearestTree();
        }
    }

    Tree getNearestTree()
    {
        var nClosest = FindObjectsOfType<Tree>().OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                                   .Take(5)   //or use .FirstOrDefault();  if you need just one
                                   .ToArray();

        nClosest = nClosest.Where(val => val != target && !lastTwoTrees.Contains(val)).ToArray(); // remove unpossible trees
        return nClosest[0];
    }
}
