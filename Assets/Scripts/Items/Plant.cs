﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;
    [SerializeField] ItemBase plant;
    int growState = 0;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[0];
    }

    public void Grow()
    {
        growState = Mathf.Clamp(growState+1, 0, sprites.Count-1);
        print(growState);
        UpdateSprite();
    }

    public void Take(Player player)
    {
        if(growState == sprites.Count-1)
            player.inventory.Add(plant);
        growState = 0;
        UpdateSprite();
    }

    void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[growState];
    }
}
