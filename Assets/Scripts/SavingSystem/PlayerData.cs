using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public float[] position;
    public bool firstLaunch;
    // public int storyProgress;
    public bool firstinventory;

    public PlayerData(Player player)
    {
        firstLaunch = player.isFirstLaunch;

        if (firstLaunch)
        {
            // new playerdata
            health = 10;
            position = new float[3];
            position[0] = -2;
            position[1] = 1.3f;
            position[2] = 0;

            firstLaunch = true;
        }
        else
        {
            health = player.hp;

            position = new float[3];
            position[0] = player.transform.position.x;
            position[1] = player.transform.position.y;
            position[2] = player.transform.position.z;
            firstinventory = GameController.Instance.storyController.FirstTime_inventory;
        }
    }

    public PlayerData() // empty
    {
        health = 10;
        position = new float[3];
        position[0] = -2;
        position[1] = 1.3f;
        position[2] = 0;

        firstLaunch = true;
    }

    public static PlayerData emtpy => new PlayerData();
}
