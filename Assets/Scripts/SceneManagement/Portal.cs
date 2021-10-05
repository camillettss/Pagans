using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Locations { FadsHeimr, AsbjarnarHeimr };

public class Portal : MonoBehaviour
{
    public Locations destination;
    [SerializeField] Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);
        print($"destPortal: {destPortal.name}, this {name}");
        GameController.Instance.player.transform.position = destPortal.spawnPoint.position;
    }
}
