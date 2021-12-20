using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public float[] position;
    public bool firstLaunch;

    // preferences
    public bool enableDiagonalMoves;

    // story

    // inventory
    public int kents;


    public PlayerData(Player player)
    {
        health = player.hp;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        firstLaunch = GameController.Instance.storyController.firstLaunch;
        enableDiagonalMoves = player.enableDiagonalMovements;

        kents = player.kents;
    }

    public PlayerData() // empty
    {
        health = 10;
        position = new float[3];
        position[0] = 237;
        position[1] = -31;
        position[2] = 0;

        firstLaunch = true;
        enableDiagonalMoves = false;
        kents = 0;
    }

    public static PlayerData emtpy => new PlayerData();
}
