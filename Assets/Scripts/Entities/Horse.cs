using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Horse : IAnimal
{
    protected override void nonTamedInteraction()
    {
        startRide();
        print("no tu non mi conosci però parli di me");
    }

    protected override void tamedInteraction()
    {
        print("oh ciao");
    }

    void startRide()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        Player.i.transform.position = transform.position;
    }
}
