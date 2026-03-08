
using System;
using UnityEngine;

public class FalseKnight : MonoBehaviour
{
    //stores the false knights states
    private enum State { Walking, Jumping, Stunned }
    private State currentState = State.Walking;

    //references to components
    private Rigidbody2D rb;
    private Enemy enemy;
    private GameObject player;
    private BoxCollider2D falseKnightCollider;

    //sets a value to all of numbers refered to multiple times
    private float walkSpeed = 4f;
    private float jumpForce = 10f;
    private float stunDuration = 3f;
    private float jumpCooldown = 3f;
    private int hitsToStun = 3;

    //the time while the boss is in the stunned state
    private float stateTimer = 0f;
    //the time before the boss is allowed to jump
    private float jumpTimer = 0f;
    //checks to see if the boss is on the ground or in the air
    private bool grounded = false;
    //checks if the boss has slammed yet
    private bool hasSlammed = false;
    //number of hits before the player can deal damage to the boss
    private int hitCounter = 0;
    //stores the direction for the boss to jump depending on the player position
    private float jumpDirectionX = 0f;

    //sprites assigned to each action
    public Sprite walkSprite;
    public Sprite jumpSprite;
    public Sprite stunSprite;

    //the shapes of the collider depending on the current state
    private Vector2 walkColliderSize = new Vector2(2.406211f, 2.772774f);
    private Vector2 walkColliderOffset = new Vector2(0.7031057f, -0.3863873f);
    private Vector2 slamColliderSize = new Vector2(3.233968f, 2.702616f);
    private Vector2 slamColliderOffset = new Vector2(1.333169f, -0.351308f);
    private Vector2 stunColliderSize = new Vector2(3.601521f, 2.059415f);
    private Vector2 stunColliderOffset = new Vector2(0.09270853f, -1.331429f);


    //variable to store the permanent defeat
    public static bool falseKnightDefeated = false;
    public ArenaActivate arenaActivate;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //stops the dynamic body causing the player to be able to push the boss around
        rb.mass = 1000f;
        rb.freezeRotation = true;
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("KnightObject");
        falseKnightCollider = GetComponent<BoxCollider2D>();

        //counts the hits before the player can deal actual damage
        enemy.onHit += RegisterHit;

        //the false knight is immidiately destroyed if he already was in a previous playsession
        if (falseKnightDefeated)
        {
            Destroy(gameObject);
            return;
        }
        //records death to store in a save file
        enemy.onDeath += () => falseKnightDefeated = true;

        //the boss starts in an invulnerable state and becomes vulnerable when stunned
        enemy.invulnerable = true;

        jumpTimer = jumpCooldown;
        SetState(State.Walking);
    }

    void Update()
    {
        if (arenaActivate == null || !arenaActivate.bossActivate)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == State.Stunned)
        {
            stateTimer -= Time.deltaTime;

            //stops the false knight from moving while stunned
            rb.linearVelocity = Vector2.zero;

            //when the timer is done the false knight will return to walking
            if (stateTimer <= 0f)
                SetState(State.Walking);
            return;
        }

        jumpTimer -= Time.deltaTime;

        //walks towards the player while the jump cooldown is expiring
        if (currentState == State.Walking)
        {
            MoveTowardPlayer();
            FlipTowardPlayer();

            if (jumpTimer <= 0f)
                SetState(State.Jumping);
        }

        //doesent flip the sprite while jumping to avoid the boss getting stuck above the player
        if (currentState == State.Jumping)
        {
            if (grounded && hasSlammed)
            {
                hasSlammed = false;
                jumpTimer = jumpCooldown;
                SetState(State.Walking);
            }
        }
    }

    //moves the boss horizontally toward the player at the enemies walkspeed
    void MoveTowardPlayer()
    {
        float direction = player.transform.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
    }

    //flips the sprite so that it faces the player
    void FlipTowardPlayer()
    {
        float direction = player.transform.position.x > transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(direction > 0 ? 3f : -3f, 3f, 1f);
    }


    void SetState(State newState)
    {
        currentState = newState;
        //all states will change the false knights collider size and the sprite that is currently shown
        if (newState == State.Walking)
        {
            enemy.invulnerable = true;
            falseKnightCollider.size = walkColliderSize;
            falseKnightCollider.offset = walkColliderOffset;
            GetComponent<SpriteRenderer>().sprite = walkSprite;
        }

        else if (newState == State.Jumping)
        {
            enemy.invulnerable = true;
            falseKnightCollider.size = slamColliderSize;
            falseKnightCollider.offset = slamColliderOffset;
            hasSlammed = false;
            GetComponent<SpriteRenderer>().sprite = jumpSprite;

            //the knight will jump towards the player
            jumpDirectionX = player.transform.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(jumpDirectionX * walkSpeed * 1.5f, jumpForce);
        }

        else if (newState == State.Stunned)
        {
            //the false knight can only take damage while stunned
            enemy.invulnerable = false;
            falseKnightCollider.size = stunColliderSize;
            falseKnightCollider.offset = stunColliderOffset;
            stateTimer = stunDuration;

            //resets the hit counter to zero so the player will have to perform another set of hits
            hitCounter = 0;
            rb.linearVelocity = Vector2.zero;
            GetComponent<SpriteRenderer>().sprite = stunSprite;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            //when touching the ground the state will return to walking
            if (currentState == State.Jumping)
                hasSlammed = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = false;
    }

    public void RegisterHit()
    {
        //ignores hits while already stunned since the player will deal health based damage
        if (currentState != State.Stunned)
        {
            hitCounter++;
            if (hitCounter >= hitsToStun)
                SetState(State.Stunned);
        }
    }
}
