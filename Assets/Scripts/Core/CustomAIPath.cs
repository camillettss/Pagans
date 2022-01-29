using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CustomAIPath : AIPath
{
    public bool alreadyCalledTargetReached = false;
    public override void OnTargetReached()
    {
        if(!alreadyCalledTargetReached)
            if (gameObject.TryGetComponent(out FarmAnimal animal))
            {
                animal.OnFemaleReached();
                alreadyCalledTargetReached = true;
            }
    }
}
