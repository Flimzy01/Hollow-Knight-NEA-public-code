using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WriteSaveData
{
    //stores save files in the correct folder
    private static string savePath => Application.persistentDataPath + "/save.json";

    public static void Save(GameObject player)
    {
        SaveData data = new SaveData();

        //all important player information
        data.health    = PlayerHealth.health;
        data.maxHealth = PlayerHealth.maxHealth;
        data.soul = PlayerStats.soul;
        data.geo  = GeoScript.geo;

        //stores the position of the last bench they interacted with
        data.positionX = player.transform.position.x;
        data.positionY = player.transform.position.y;

        //stores the player inventory of bought items and what charms they have equipped
        data.charmInventory = new List<string>(PlayerStats.charmInventory);
        data.inventory = new List<string>(PlayerStats.inventory);

        //stores which bosses are defeated
        data.gruzMotherDefeated = GruzMotherAttack.gruzMotherBossDefeated;
        data.falseKnightDefeated = FalseKnight.falseKnightDefeated;

        //stores all of the doors broken so that the player doesent get stuck on scene transitions
        data.brokenDoors = new List<string>(DestructibleObject.brokenDoors);

        //stores the last scene the player was on
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        //serializes and writes to json
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(savePath, json);
    }


    public static SaveData Load()
    {
        //all file save data is deserilized to be used when the save file is loaded
        if (!File.Exists(savePath))
        {
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    //true if there is a save file on the players hard drive
    public static bool SaveExists() => File.Exists(savePath);
}