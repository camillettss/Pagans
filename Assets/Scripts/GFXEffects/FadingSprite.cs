using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class FadingSprite : InstanceTracker<FadingSprite>
{
    internal SpriteRenderer sp;
    internal float alpha = 1, velocity, targetAlpha = 1;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            targetAlpha = .5f;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            targetAlpha = 1f;
    }
}
