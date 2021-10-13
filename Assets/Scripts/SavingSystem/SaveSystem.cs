﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Add different save files.
public static class SaveSystem
{
    public static void SavePlayer(Player player)
    {
        SavePlayer(new PlayerData(player));
    }

    static void SavePlayer(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer ()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        } 
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void Reset()
    {
        SavePlayer(PlayerData.emtpy);
    }
}
