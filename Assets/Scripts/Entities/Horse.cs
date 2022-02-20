using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : IAnimal
{
    protected override void nonTamedInteraction()
    {
        print("no tu non mi conosci però parli di me");
    }

    protected override void tamedInteraction()
    {
        print("oh ciao");
    }
}
