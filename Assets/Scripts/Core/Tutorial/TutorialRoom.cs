using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TutorialRoom : MonoBehaviour
{
    [SerializeField] RoomsController controller;
    public int level;

    public bool alreadyEntered = false;
    public bool alreadyLeft = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            controller.triggered(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            controller.exits(this);
    }
}
