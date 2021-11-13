using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEventHandler : MonoBehaviour
{
    public void Interact()
    {
        if (Player.i.activeAltar != null)
            StartCoroutine(Player.i.activeAltar.Use());
        else
        {
            var front = Player.i.GetFrontalCollider();
            if (front == null)
                return;

            if(front.TryGetComponent(out IEntity entity))
            {
                entity.Interact(Player.i);
                StartCoroutine(waitForState(refreshQuest));
            }
        }
    }

    int refreshQuest()
    {
        if (Player.i.quest != null) // Doesn't work, it always update cuz count is always >0.
        {
            print("aggiornamo tutto frate");
        }
        return 0;
    }

    IEnumerator waitForState(Func<int> f, GameState state = GameState.FreeRoam)
    {
        while(true)
        {
            if (GameController.Instance.state != state)
                yield return 0;
            else
                break;
        }
        f();
    }
}