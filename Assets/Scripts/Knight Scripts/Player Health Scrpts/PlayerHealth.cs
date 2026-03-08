using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    //when the player takes damage causes the hearts to be redrawn
    public static event Action OnPlayerDamaged;

    //current player state of health and healing
    public static float health, maxHealth;
    public static int healthPerHeal = 1;

    //invincibility frames when damaged;
    private float invincibilityTimer = 0f;
    private float invincibilityDuration = 1.5f;

    //counts down until zero when the heal is initiated
    private int counter = PlayerStats.healCounter;

    //noise played when the player takes damage
    public AudioClip damage;


    void Start()
    {
        //initialises health
        maxHealth = 5;
        health = maxHealth;
    }

    //sets health to a given value and redraws it
    public void SetHealth(int pHealth)
    {
        health = pHealth;
        OnPlayerDamaged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        //if the player has invincibility taking damage wont occur
        if (invincibilityTimer > 0f)
        return;

        //loses health and begins the invincibility timer
        health -= amount;
        invincibilityTimer = invincibilityDuration;
        OnPlayerDamaged?.Invoke();

        //plays the damage audio
        AudioSource.PlayClipAtPoint(damage, transform.position);

        //when health is less than zero the player is taken back to their spawnpoint and has max health again
        if (health <= 0)
        {
            health = 0;
            this.transform.position = PlayerStats.respawnPoint.transform.position;
            health = maxHealth;
            OnPlayerDamaged?.Invoke();
        }
    }

    //refreshes the UI without taking damage
    public static void RefreshHealth()
    {
        OnPlayerDamaged?.Invoke();
    }

    
    void FixedUpdate()
    {
        //checks for a healing action
        if (Keyboard.current.sKey.isPressed && PlayerStats.soul >= 33)
            counter--;

        //redraws the health when the player sits on a bench
        if (BenchCheck.satOnBench)
            OnPlayerDamaged?.Invoke();
        
    }
    void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;

            // ignore collision with enemies while invincible
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), invincibilityTimer > 0f);
        }

        //checks for key to begin healing
        if (GetPressedKeys().Contains(Keyboard.current.sKey))
        {
            //if any other keys are pressed process stops and counter is reset
            if (GetPressedKeys().Contains(Keyboard.current.upArrowKey))
                return;
            else if (GetPressedKeys().Contains(Keyboard.current.leftArrowKey))
                return;
            else if (GetPressedKeys().Contains(Keyboard.current.downArrowKey))
                return;
            else if (GetPressedKeys().Contains(Keyboard.current.upArrowKey))
                return;
            else if (GetPressedKeys().Contains(Keyboard.current.aKey))
                return;
            else if (GetPressedKeys().Contains(Keyboard.current.xKey))
                return;

            //performs the heal when the counter is zero
            if (counter == 0)
            {
                PlayerStats.soul -= 33;
                health = Mathf.Min(health + healthPerHeal, maxHealth);
                counter = PlayerStats.healCounter;
                OnPlayerDamaged?.Invoke();
            }


            if (!GetPressedKeys().Contains(Keyboard.current.sKey))
                counter = PlayerStats.healCounter;

            //syncs the counter with if the player has the quickheal charm equipped
            if (PlayerStats.healCounter != counter)
            {
                counter = PlayerStats.healCounter;
            }
        }
    }

    //Google AI helper method
    //returns a list of every key the player has pressed
    public List<KeyControl> GetPressedKeys()
    {
        var pressedKeys = new List<KeyControl>();
        var keyboard = Keyboard.current;

        if (keyboard == null) return pressedKeys;

        foreach (var key in keyboard.allKeys)
        {

            if (key != null && key.isPressed)
            {
                pressedKeys.Add(key);
            }
        }

        return pressedKeys;
    }
}

