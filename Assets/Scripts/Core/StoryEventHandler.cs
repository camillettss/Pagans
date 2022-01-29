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

    public delegate void Del(NPCController entity, InteractionType type); // crea un delegato che incapsula un metodo accettante un controller e restituente void

    public static void DelegateMethod(NPCController entity, InteractionType type) // crea un metodo con gli stessi parametri del delegato
    {
        if(type == InteractionType.talkTo)
            if(Player.i.quest != null && Player.i.quest.goal.Count > 0)
                Player.i.quest.goal[0].NPCTalked(entity);
    }

    public void Interact()
    {
        if(Player.i.activePort != null)
        {
            Player.i.activePort.Choose();
        }
        else if (Player.i.activeAltar != null)
            StartCoroutine(Player.i.activeAltar.Use());

        else if(Player.i.activePlant != null)
        {
            Player.i.ShowPlantDetails();
        }
        else if (Player.i.liftingItem != null)
        {
            StartCoroutine(Player.i.liftingItem.Throw());
        }
        else
        {
            Del handler = DelegateMethod; // crea un istanza del delegato
            var front = Player.i.GetFrontalCollider();
            if (front == null)
            {
                return;
            }

            if(front.TryGetComponent(out TraderController trader))
            {
                trader.Interact(Player.i); // coroutine solo per le entità con dialogo.
            }
            else if(front.TryGetComponent(out NPCController npc))
            {
                npc.Interact(Player.i); // chiama npcTalked per gli npc
                StartCoroutine(waitForState(handler, npc)); // passa a waitForState il delegato istanziato e i parametri per chiamarlo
            }
            else if(front.TryGetComponent(out IEntity entity))
            {
                entity.Interact(Player.i); // interazione per le entità generiche.
            }
        }
    }

    public void OnEquip(ItemBase item)
    {
        if(Player.i.quest != null && Player.i.quest.goal.Count>0)
            Player.i.quest.goal[0].SomethingEquiped(item);
    }

    public IEnumerator GoalCompleted(Quest quest)
    {
        // prima di iniziare il prossimo goal chiama il dialogo
        print($"started GoalCompleted. first goal: {quest.goal[0].goal}");
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

    public void AddToInventory(ItemBase item, int quantity=1)
    {
        Player.i.inventory.Add(item, quantity);
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

        CameraMover.i.transform.position = Player.i.transform.position;
        Player.i.GetComponent<SpriteRenderer>().DOFade(1f, .1f);
        yield return Fader.i.FadeOut(.5f);
        Player.i.teleporting = false;
    }

    IEnumerator waitForState(Del callback, NPCController entity=null, InteractionType type=InteractionType.talkTo, GameState state = GameState.FreeRoam)
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