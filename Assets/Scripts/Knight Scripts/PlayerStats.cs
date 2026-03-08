using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //combat information before stat changes due to charms
    public static float baseDamage = 6;
    public static float damage;              
    public static int fireBallDamage = 18;

    //soul for spell casts
    public static int soul = 0;
    public static int maxSoul = 99;

    //shop items bought
    public static List<string> inventory = new List<string>();

    //currently equipped charms
    public static List<string> charmInventory = new List<string>();

    public static Vector2 lastBench = Vector2.zero;
    public static GameObject respawnPoint;

    //long Nail charm to extend the attack hitbox
    public static bool longAttack;  
    
    //quick slash charm to reduce attack duration
    public static bool quickAttack;

    // ----- Healing -----
    //number of frames the player must hold the skey before healing triggers
    public static int healCounter;

    //avoids re-triggering the mask upgrade sequence
    private bool maskUpgradeApplied = false;

    void Start()
    {
        respawnPoint = GameObject.Find("PlayerRespawn");
    }

    void FixedUpdate()
    {
        //stops the soul going over its maximum
        if (soul > maxSoul)
            soul = maxSoul;

        //move the respawn point to the last bench location when it changes
        if (lastBench != Vector2.zero && respawnPoint != null)
        {
            respawnPoint.transform.position = new Vector3(lastBench.x, lastBench.y, 0);
            
            //no need to keep repositionioning the spawn point
            lastBench = Vector2.zero; 
        }

        //apply charm effects so changes take effect instantly
        ApplyCharmEffects();

        //apply shop upgrades only once
        ApplyShopPurchases();
    }

    //recalculates stats based on equipped charms
    public static void ApplyCharmEffects()
    {
        //resets to base values before applying modifiers
        damage = baseDamage;
        PlayerHealth.healthPerHeal = 1;
        healCounter = 150;
        quickAttack = false;
        longAttack = false;

        foreach (string charmName in charmInventory)
        {
            if (charmName == "Heavy Blow")
                damage = baseDamage * 1.5f;
            if (charmName == "Deep Focus")
                PlayerHealth.healthPerHeal = 2;
            if (charmName == "Quick Focus")
                healCounter = 75;
            if (charmName == "Quick Slash")
                quickAttack = true;
            if (charmName == "Long Nail")  
                longAttack  = true;
        }
    }

    //
    public void ApplyShopPurchases()
    {
        //if it is already applied no need to apply again
        if (maskUpgradeApplied) 
            return;

        foreach (string itemBought in inventory)
        {
            if (itemBought == "PlayerMask")
            {
                //permanently increase max health and refresh the display
                PlayerHealth.maxHealth = 6;
                
                //only increase health if it is less than the new maximum
                if (PlayerHealth.health < PlayerHealth.maxHealth)
                    PlayerHealth.health = PlayerHealth.maxHealth;

                PlayerHealth.RefreshHealth();
                
                //prevent re-running next FixedUpdate
                maskUpgradeApplied = true;
            }
        }
    }
}