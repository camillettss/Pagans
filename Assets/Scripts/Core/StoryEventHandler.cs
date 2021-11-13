using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    talkTo
}

public class StoryEventHandler : MonoBehaviour
{

    public delegate void Del(IEntity entity, InteractionType type); // crea un delegato che incapsula un metodo accettante un controller e restituente void

    public static void DelegateMethod(IEntity entity, InteractionType type) // crea un metodo con gli stessi parametri del delegato
    {
        if(type == InteractionType.talkTo)
        {
            try
            {
                Player.i.quest.goal[0].NPCTalked((NPCController)entity);
            }
            catch { };
        }
    }

    public void Interact()
    {
        if (Player.i.activeAltar != null)
            StartCoroutine(Player.i.activeAltar.Use());
        else
        {
            Del handler = DelegateMethod; // crea un istanza del delegato
            var front = Player.i.GetFrontalCollider();
            if (front == null)
                return;

            if(front.TryGetComponent(out IEntity entity))
            {
                entity.Interact(Player.i);
                StartCoroutine(waitForState(handler, entity)); // passa a waitForState il delegato istanziato e i parametri per chiamarlo
            }
        }
    }

    IEnumerator waitForState(Del callback, IEntity entity=null, InteractionType type=InteractionType.talkTo, GameState state = GameState.FreeRoam)
    {
        while(true)
        {
            if (GameController.Instance.state != state)
                yield return 0;
            else
                break;
        }
        callback(entity, type); // quando torna allo stato aspettato, chiama il callback delegato.
    }
}