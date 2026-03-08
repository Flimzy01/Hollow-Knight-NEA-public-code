using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction playerControls;

    //players initial movement initialised to zero
    Vector2 moveDirection = Vector2.zero;

    //base movement multipliers
    public float speed = 2f;
    public float jumpStrength = 5f;

    //variables and conditions related to the ability to jump
    public static Boolean grounded;
    private int jumping = 0;
    public static int jumpTime = 3;

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //disables all input while on a bench
        if (BenchCheck.satOnBench)
            return;

        moveDirection = playerControls.ReadValue<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //while colliding with the ground the player is able to jump
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            jumping = 0;
        }

        //when they collide with a spike they will be pushed off it
        if (other.gameObject.CompareTag("Spike"))
        {
            float finalVelocityx = rb.linearVelocity.x + 30f;
            float finalVelocityy = rb.linearVelocity.y + 30f;
            rb.linearVelocity = new Vector2(finalVelocityx, finalVelocityy);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            //stops player knockback while swinging
            if (Swing.isSwinging)
            return;


            //player will recieve knockback depending on the direction the enemy is travelling in
            float rawX = other.gameObject.GetComponent<Rigidbody2D>().linearVelocity.x;
            if (rawX < 0f)
                rb.linearVelocity = new Vector2(-50f, 10f);
            else
                rb.linearVelocity = new Vector2(50f, 10f);
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        //true while the player stays on the ground
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //when the player leaves the ground they no longer have the ability to jump again
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //stops the players swing from causing knockback
        if (collision.transform.IsChildOf(this.transform))
            return;

        if (Swing.isSwinging)
            return;

    }


    private void FixedUpdate()
    {
        //no movement options while sat on a bench
        if (BenchCheck.satOnBench)
            return;

        //the ability to jump is once again avaialable when the player lands on the ground and isnt holding jump
        if (grounded && !Keyboard.current.upArrowKey.isPressed)  
            jumping = 0;

        float moveThisFramey = 0;
        float moveThisFramex = 0;

        //changes the height traveled depending on the number of frames the player holds the up arrow
        if (Keyboard.current.upArrowKey.isPressed && (jumping <= jumpTime || grounded))
        {
            moveThisFramey = jumpStrength * speed;
            jumping++;
        }

        //player horizontal movement always locked at a set speed
        if (Keyboard.current.leftArrowKey.isPressed)
            moveThisFramex += -speed * 2f;
        
        if (Keyboard.current.rightArrowKey.isPressed)
            moveThisFramex += speed * 2f;
        
        //applies and dampens the final velocity
        float finalVelocityx = rb.linearVelocity.x + moveThisFramex;
        finalVelocityx *= 0.92f;

        //limits the velocity of the player to be between -10f and 10f
        if (Mathf.Abs(finalVelocityx) > 10f)
        {
            if (finalVelocityx < 0)
                finalVelocityx = -10f; 
            else
                finalVelocityx = 10f;  
        }

        //adds the jump velocity in addition to the gravity acting on the player
        float finalVelocityy = rb.linearVelocity.y + moveThisFramey;
        rb.linearVelocity = new Vector2(finalVelocityx, finalVelocityy);
    }

}