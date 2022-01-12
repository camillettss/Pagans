using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgribleTile : MonoBehaviour
{
    public Seeds seed = null;

    int state = 0;

    SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.sprite = seed.growLevels[state];
    }

    public void Grow()
    {
        if(seed != null)
        {
            state++;
            sp.sprite = seed.growLevels[state];
        }
    }

    public void Plant(Seeds seed)
    {
        this.seed = seed;
        state = 0;

        sp.sprite = seed.growLevels[0];
    }
}
