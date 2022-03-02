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

    public int daysToGrow;
    public int daysPassed;

    // per determinare il range
    [SerializeField] int sub_variation;
    [SerializeField] int up_variation;

    SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();

        if (seed != null)
            sp.sprite = seed.growLevels[state];
        else
        {
            sp.sprite = noSeedSprite;
        }

        daysToGrow = Random.Range(daysToGrow - sub_variation, daysToGrow + up_variation);
    }

    public void Grow()
    {
        if(seed != null)
        {
            state++;
            state = Mathf.Clamp(state, 0, seed.growLevels.Count - 1);

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

    void EvalState(int state)
    {
        state = Mathf.Clamp(state, 0, seed.growLevels.Count - 1);
        if(state != this.state)
        {
            this.state = state;
            sp.sprite = seed.growLevels[state];
        }
    }

    public void NewDay()
    {
        if(seed != null)
        {
            daysPassed++;

            if (daysPassed == daysToGrow / 2)
                EvalState(1);
            else if (daysPassed == daysToGrow)
                EvalState(2); // attualmente hardcoded, servono 3 sprite totali.
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player.i.activePlant = this;
            GameController.Instance.plantDetailsUI.UpdateUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player.i.activePlant = null;
            GameController.Instance.plantDetailsUI.UpdateUI();
        }
    }
}
