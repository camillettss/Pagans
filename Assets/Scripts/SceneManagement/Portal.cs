using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Locations { Shop };

public class Portal : MonoBehaviour
{
    public Locations destination;
    [SerializeField] Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);
        print($"destPortal: {destPortal}, this {this}");
        GameController.Instance.player.transform.position = destPortal.spawnPoint.position;
    }
}
