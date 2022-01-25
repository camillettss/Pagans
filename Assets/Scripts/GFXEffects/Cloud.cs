using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : InstanceTracker<Cloud>
{
    public Vector3 direction;
    float speed = 1.5f;

    public void HandleUpdate() // called by a fixed update
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(transform.position, Player.i.transform.position) > 15)
        {
            gameObject.SetActive(false);
        }
    }
}
