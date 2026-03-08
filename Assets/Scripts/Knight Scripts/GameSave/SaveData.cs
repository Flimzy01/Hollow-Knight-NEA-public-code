using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    //stores if a boss was defeated or not
    public bool gruzMotherDefeated;
    public bool falseKnightDefeated; 
    //stores all information that the player uses during runtime
    public float health;
    public float maxHealth;
    public int soul;
    public int geo;

    //x and y position of the last bench the player was at
    public float positionX;
    public float positionY;

    //stores the names of equipped charms
    public List<string> charmInventory = new List<string>();
    //stores the names of items bought from the shop
    public List<string> inventory = new List<string>();

    //stores the names of all broken doors
    public List<string> brokenDoors = new List<string>();
}