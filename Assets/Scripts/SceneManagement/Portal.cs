using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;
using DG.Tweening;

public enum Locations { FadsHeimr, AsbjarnarHeimr, TutorialShop, FarmHouse, MagicianHouse, Cave1, Library,
    // breidr
    BreidrHouse1,
    BreidrHouse2,
    // fornburg
    FornburgHouse,
    FornburgBigHouse,
    FornburgForge,
    // furnes
    FurnesHouse1,
    FurnesBigHouse_Ingress,
    FurnesBigHouse_Room,
    FurnesMansion_Ingress,
    FurnesMansion_Room,
    // fornburg
    FornburgHouse_SecretGate,
    // Caves
    FornburgCave_South,
    FornburgCave_North,
    // Hruttstalidr
    Hruttstalidr_Forge,
    Hruttstalidr_Storage,
    Hruttstalidr_FarmadsHeimr,
    Hruttstalidr_HeirdaHeimr,
    Hruttstalidr_GodaHeimr,
    Hruttstalidr_FarmerHouse1,
    Hruttstalidr_FarmerHouse2,
    Hruttstalidr_FarmerHouse3,
    Hruttstalidr_FarmerHouse4,
    Hruttstalidr_Biblioteca,
};
public enum PortalDetails { IN, OUT, NONE };

public class Portal : MonoBehaviour
{
    public Locations destination;
    public PortalDetails details;

    public Transform spawnPoint;
    public Transform AISpawnPoint;

    [ConditionalField(nameof(destination), false, Locations.Library)] public bool goingIn = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);

        if(collision.tag == "Player")
        {
            if (Player.i.liftingItem != null)
                StartCoroutine(Player.i.liftingItem.Throw(true));

            Player.i.teleporting = true;
            Player.i.GetComponent<SpriteRenderer>().DOFade(0f, .3f);
            StartCoroutine(GameController.Instance.EvH.changeScene(destPortal));
        }
        else
        {
            if(collision.TryGetComponent(out NPCController npc))
            {
                npc.transform.position = destPortal.AISpawnPoint.position;
            }
            if(collision.TryGetComponent(out NoAIBehaviour ai)) // if has NoAI component too
            {
                ai.onEnterDoor(this);
            }
        }
    }
}
