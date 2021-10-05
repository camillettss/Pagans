using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public float[] position;
    public bool firstLaunch;

    public PlayerData(Player player)
    {
        health = player.hp;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        firstLaunch = GameController.Instance.storyController.firstLaunch;
    }
}
