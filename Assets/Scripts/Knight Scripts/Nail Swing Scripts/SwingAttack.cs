using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;


public class Swing : MonoBehaviour
{
    //the objects for attack and the player
    private GameObject Attack;
    private GameObject KnightObject;

    //variables which effect attack timing
    private static int attackCooldown = 15;
    public static int attackDuration = 0;

    //attack directional states
    private bool downKey = false;
    private bool upKey = false;

    //to find the players direction that they are facing
    public static bool lastDirection = false;

    //stores all enemies hit in a single swing of the attack to stop multihits
    private HashSet<Collider2D> hitThisSwing = new HashSet<Collider2D>();

    //used by movementscript
    public static bool isSwinging = false;

    //audio sound effect for an attack
    public AudioClip swing;

    void Awake()
    {
        //resets the attack on the occurance of a sceneload
        attackCooldown = 0;
        attackDuration = 0;
        isSwinging = false;
        lastDirection = false;

        Attack = GameObject.Find("Attack");
        KnightObject = GameObject.Find("KnightObject");

        Attack.SetActive(false);
    }
    public void FixedUpdate()
    {
        //cannot attack while on a bench
        if (BenchCheck.satOnBench)
            return;

        //changes the scale of the attack if the longattack charm is equipped
        if (PlayerStats.longAttack)
        {
            Attack.transform.localScale = new(3, 4.5f);
        }

        //finds the last direction so while not in attack the player can change the direction of the next swing
        if (Keyboard.current.leftArrowKey.isPressed && attackDuration <= 0)
            lastDirection = false;

        else if (Keyboard.current.rightArrowKey.isPressed && attackDuration <= 0)
            lastDirection = true;

        //rotates the hitbox downwards if the downkey is pressed before the swing occurs
        if (Keyboard.current.downArrowKey.isPressed && !downKey && attackDuration <= 0)
        {
            Attack.transform.localScale = PlayerStats.longAttack ? new(4.5f, 6) : new(3, 4.5f);

            //rotates the attack depending on the current direction due to inversion of scaling
            if (lastDirection)
                Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, -90);
            else if (!lastDirection)
                Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, 90);

            downKey = true;
        }

        //rotates the hitbox downwards if the upkey is pressed before the swing occurs
        else if (Keyboard.current.upArrowKey.isPressed && !upKey && attackDuration <= 0)
        {
            Attack.transform.localScale = PlayerStats.longAttack ? new(4.5f, 6) : new(3, 4.5f);

            //rotates the attack depending on the current direction due to inversion of scaling
            if (lastDirection)
                Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, 90);
            else if (!lastDirection)
                Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, -90);

            upKey = true;
        }

        //starts swinging when the xkey is pressed and the cooldown is finished
        if (Keyboard.current.xKey.isPressed && attackCooldown == 0)
        {
            //quickattack ensures a shorter swing and cooldown
            if (PlayerStats.quickAttack)
            {
                attackCooldown = 5;
                attackDuration = 15;
            }
            else
            {
                attackCooldown = 15;
                attackDuration = 25;
            }

            Attack.SetActive(true);
            //resets what the swing can hit
            hitThisSwing.Clear();
            isSwinging = true;
            AudioSource.PlayClipAtPoint(swing, transform.position);
        }

        //hides the hitbox while the swing is not active
        if (attackDuration == 0)
        {
            Attack.SetActive(false);
            isSwinging = false;
        }

        //only starts cooldown when the attack is finished
        if (attackDuration < 0)
            attackCooldown--;

        attackDuration--;

        //stops cooldown from becoming negative
        if (attackCooldown < 0)
            attackCooldown = 0;

        //resets the down rotation once the swing is over
        if (downKey && attackDuration <= 0 && lastDirection)
        {
            Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, 90);
            downKey = false;
            Attack.transform.localScale = PlayerStats.quickAttack ? GetQuickAttackScale(false) : GetAttackScale(false);
        }
        else if (downKey && attackDuration <= 0 && !lastDirection)
        {
            Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, -90);
            downKey = false;
            Attack.transform.localScale = PlayerStats.quickAttack ? GetQuickAttackScale(false) : GetAttackScale(false);
        }

        //resets the up rotation once the swing is over
        if (upKey && attackDuration <= 0 && lastDirection)
        {
            Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, -90);
            upKey = false;
            Attack.transform.localScale = PlayerStats.quickAttack ? GetQuickAttackScale(false) : GetAttackScale(false);
        }
        else if (upKey && attackDuration <= 0 && !lastDirection)
        {
            Attack.transform.RotateAround(KnightObject.transform.position, Vector3.forward, 90);
            upKey = false;
            Attack.transform.localScale = PlayerStats.quickAttack ? GetQuickAttackScale(false) : GetAttackScale(false);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    private void HandleHit(Collider2D collision)
    {
        if (hitThisSwing.Contains(collision))
            return;

        hitThisSwing.Add(collision);

        //mosscharger has a different set of conditions to take damage than a normal enemy
        if (collision.TryGetComponent<MossCharger>(out MossCharger mossCharger))
        {
            mossCharger.TryTakeDamage((int)PlayerStats.damage);
            //gain soul on a hit
            PlayerStats.soul += 11;
            return;
        }

        //standard enemy attack collision
        if (collision.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            enemyComponent.TakeDamage((int)Math.Ceiling(PlayerStats.damage));
            PlayerStats.soul += 11;
        }

        //destroyable objects will recieve a set damamge for how many times they need to be hit to break
        if (collision.TryGetComponent<DestructibleObject>(out DestructibleObject destructibleComponent))
        {
            destructibleComponent.TakeDamage(1);
        }
    }

    //returns the attack scale for normal and rotated swings with or without the long nail charm equipped
    private Vector3 GetAttackScale(bool rotated)
    {
        float baseX = rotated ? 3f : 3f;
        float baseY = rotated ? 4.5f : 3f;

        if (PlayerStats.longAttack)
        {
            baseX += 1.5f;
            baseY += 1.5f;
        }

        return new Vector3(baseX, baseY);
    }

    private Vector3 GetQuickAttackScale(bool rotated)
    {
        float baseX = rotated ? 4.5f : 3f;
        float baseY = rotated ? 6f : 3f;

        if (PlayerStats.longAttack)
        {
            baseX += 1.5f;
            baseY += 1.5f;
        }

        return new Vector3(baseX, baseY);
    }


}



