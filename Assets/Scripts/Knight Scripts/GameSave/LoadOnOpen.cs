using System.Collections.Generic;
using UnityEngine;

public class LoadOnOpen : MonoBehaviour
{
    void Start()
    {
        //if no save file exists a new one is made
        if (!WriteSaveData.SaveExists())
        return;

        SaveData data = WriteSaveData.Load();
        
        //if the file is corrupted or cant be found, stops the process from crashing
        if (data == null)
        return;

        //all player values are stored to be reloaded
        PlayerHealth.health = data.health;
        PlayerHealth.maxHealth = data.maxHealth;
        PlayerStats.soul = data.soul;
        GeoScript.geo = data.geo;

        //stores the player inventories so they keep their equipped charm on a reload
        PlayerStats.charmInventory = data.charmInventory;
        PlayerStats.inventory = data.inventory;

        //stores the bosses the player defeated
        GruzMotherAttack.gruzMotherBossDefeated = data.gruzMotherDefeated;
        FalseKnight.falseKnightDefeated = data.falseKnightDefeated;

        //stores all of the doors broken by the player so they stay broken through scene transitions
        DestructibleObject.brokenDoors = new HashSet<string>(data.brokenDoors);

        //stores the position of the last bench
        transform.position = new Vector3(data.positionX, data.positionY, 0);
    }
}