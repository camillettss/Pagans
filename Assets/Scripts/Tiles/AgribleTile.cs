using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgribleTile : InstanceTracker<AgribleTile>
{
    public Seeds seed = null;
    public Sprite noSeedSprite;

    public Date grownDate;
    public bool isGrown = false;

    int state = 0;
    

    SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();

        if (seed)
            sp.sprite = seed.growLevels[state];
        else
            sp.sprite = noSeedSprite;
    }

    public void Grow()
    {
        if(seed != null)
        {
            state++;
            if (state == seed.growLevels.Count-1)
            {
                sp.sprite = seed.growLevels[state];
                isGrown = true;
            }
            else if(state <= seed.growLevels.Count)
            {
                sp.sprite = seed.growLevels[state];
            }

        }
    }

    public void Plow()
    {
        state = 0;
        seed = null;
        sp.sprite = noSeedSprite;
    }

    public void Plant(Seeds seed)
    {
        this.seed = seed;
        state = 0;

        sp.sprite = seed.growLevels[0];
        grownDate = GameController.Instance.calendar.GetDate(seed.daysToGrow); // add a random range to this
        print("will be grown at: " + grownDate.month+' '+grownDate.day);
    }
}
