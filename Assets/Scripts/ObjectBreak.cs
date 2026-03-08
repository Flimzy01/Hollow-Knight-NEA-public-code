using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class DestructibleObject : MonoBehaviour
{
    private Rigidbody2D rb;
    
    //the sprite that the object swaps to when the chest or geodeposit is broken
    public Sprite Reward;
    
    //health status
    private int objectHealth, maxObjectHealth;
    
    //ensures the item is launched only once
    private bool hasBeenLaunched = false;

    //data used specifically for items dropped by chests or geodeposits
    public string itemName, room;

    //names of all the door objects which have been broken
    public static HashSet<string> brokenDoors = new HashSet<string>();


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //stops doors which are already broken from spawning in again
        if (this.gameObject.CompareTag("BigDoor") && brokenDoors.Contains(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        //sets health based on the object type
        if (this.gameObject.CompareTag("Chest") || this.gameObject.CompareTag("BreakableDoor"))
            maxObjectHealth = 1;
        else if (this.gameObject.CompareTag("GeoDeposit") || this.gameObject.CompareTag("BigDoor"))
            maxObjectHealth = 3;

        //doors dont drop items
        if (this.gameObject.CompareTag("BreakableDoor") || this.gameObject.CompareTag("BigDoor"))
        {
            itemName = null;
            room = null;
        }

        objectHealth = maxObjectHealth;
    }

    //handles a player hit from the swing script
    public void TakeDamage(int TakeDamage)
    {
        objectHealth -= TakeDamage;
    }


    void FixedUpdate()
    {
        //destroys the object when the health is zero
        if (objectHealth <= 0 && (this.gameObject.tag == "BreakableDoor" || this.gameObject.tag == "BigDoor"))
        {
            if (this.gameObject.CompareTag("BigDoor"))
                brokenDoors.Add(gameObject.name);
            Destroy(this.gameObject);
        }

        //converts the item into something that can be picked up and changes the sprite
        if (objectHealth <= 0 && (this.gameObject.tag == "Chest" || this.gameObject.tag == "GeoDeposit"))
        {
            transform.localScale = new(1, 1);
            gameObject.GetComponent<SpriteRenderer>().sprite = Reward;
            this.gameObject.tag = "Pickup";

        }

        if (objectHealth <= 0 && (this.gameObject.tag == "BreakableDoor" || this.gameObject.tag == "BigDoor"))
        {
            Debug.Log("Destroying door: " + gameObject.name);
            Destroy(this.gameObject);
        }

        //launches an item with the tag pickup
        if (this.gameObject.tag == "Pickup")
            LaunchItem();
        
    }

    private void LaunchItem()
    {
        //applies velicity to launch the item when the conditions prior to it are met
        if (!hasBeenLaunched)
        {
            rb.linearVelocity = new(-3f, 20f);
            hasBeenLaunched = true;
            Debug.Log(rb.linearVelocity);
        }

        //gradually slows the item down
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.85f, rb.linearVelocity.y * 0.85f);

        //stops when the item is practically still
        if (rb.linearVelocityY <= 0.2f)
            rb.linearVelocity = Vector2.zero;
        

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //destroys any door that has an object health less than zero
        if (objectHealth <= 0 && (this.gameObject.tag == "BreakableDoor" || this.gameObject.tag == "BigDoor"))
            Destroy(this.gameObject);
    

        //collects the item for the player to pick up
        if (this.gameObject.CompareTag("Pickup") && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(PlayerStats.charmInventory.ToArray());
            Destroy(this.gameObject);
        }
    }
}
