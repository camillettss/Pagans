using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Locations { FadsHeimr, AsbjarnarHeimr, TutorialShop, FarmHouse, MagicianHouse, Cave1 };

public class Portal : MonoBehaviour
{
    public Locations destination;
    [SerializeField] Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);
        print($"destPortal: {destPortal.name}, this {name}");
        if(Player.i.quest != null && Player.i.quest.goal != null)
        {
            Player.i.quest.goal[0].DoorEntered(this);
            if (destination == Locations.Cave1)
                StartCoroutine(GameController.Instance.storyController.EnteredCave());
        }
        GameController.Instance.player.transform.position = destPortal.spawnPoint.position;

        /*
        if (goingToindoor)
            GameController.Instance.GlobalLight.intensity = 0.3f;
        else
            GameController.Instance.GlobalLight.intensity = 1f;*/
    }
}
