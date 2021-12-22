using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CityDetails : MonoBehaviour
{
    public List<Vector2> Houses;
    public Vector2 Shop;
    public Vector2 pub;

    public GameObject entitiesContainer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            Player.i.triggeredCity = this;

        else if (collision.TryGetComponent(out AIMover ai))
            ai.triggeredCity = this;
    }
}
