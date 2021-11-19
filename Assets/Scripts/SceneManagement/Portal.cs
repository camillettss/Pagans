using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public enum Locations { FadsHeimr, AsbjarnarHeimr, TutorialShop, FarmHouse, MagicianHouse, Cave1, Library };

public class Portal : MonoBehaviour
{
    public Locations destination;
    public Transform spawnPoint;
    [ConditionalField(nameof(destination), false, Locations.Library)] public bool goingIn = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);

        StartCoroutine(GameController.Instance.EvH.changeScene(destPortal));
    }
}
