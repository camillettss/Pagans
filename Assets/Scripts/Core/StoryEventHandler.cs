using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum InteractionType
{
    talkTo
}

public class StoryEventHandler : MonoBehaviour
{
    public static StoryEventHandler i;

    private void Awake()
    {
        i = this;
    }

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

    public void OnEquip(ItemBase item)
    {
        if(Player.i.quest.goal.Count>0)
            Player.i.quest.goal[0].SomethingEquiped(item);
    }

    public IEnumerator GoalCompleted(Quest quest)
    {
        // prima di iniziare il prossimo goal chiama il dialogo
        if (quest.goal[1].introDialogue.sentences.Length > 0)
        {
            yield return new WaitForSeconds(quest.goal[1].dialogueDelay);
            var actualState = GameController.Instance.state;
            GameController.Instance.dialogueBox.StartDialogue(quest.goal[1].introDialogue, () =>
            {
                GameController.Instance.state = actualState;
                quest.goal.RemoveAt(0);
                Player.i.quest = quest;
                Player.i.UpdateQuestUI();
            });
        }
    }

    public void AddToInventory(ItemBase item)
    {
        Player.i.inventory.Add(item);
        try
        {
            Player.i.quest.goal[0].SomethingAddedToInventory(item);
        }
        catch { };
    }

    public IEnumerator changeScene(Portal destPortal)
    {
        yield return Fader.i.FadeIn(.5f);

        Player.i.transform.position = destPortal.spawnPoint.position;

        if (Player.i.quest != null && Player.i.quest.goal != null)
        {
            Player.i.quest.goal[0].DoorEntered(destPortal);
        }

        Player.i.GetComponent<SpriteRenderer>().DOFade(1f, .1f);
        yield return Fader.i.FadeOut(.5f);
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