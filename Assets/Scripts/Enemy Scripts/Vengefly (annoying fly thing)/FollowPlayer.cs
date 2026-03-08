using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private GameObject fly;

    private float followSpeed = 8.5f;
    private float followRange = 30f;
    private Vector2 offset = new(0, 0);


    void Start()
    {
        player = GameObject.Find("KnightObject");
        fly = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //stops the fly from rotating, since it is a dynamic body
        transform.localEulerAngles = new Vector3(0,0,0);
    }
    void FixedUpdate()
    {
        //finds the direction in x and y relative to the player
        Vector2 direction = player.transform.position - transform.position;

        //finds the hypotenuse of the x and y
        float distanceSquared = direction.sqrMagnitude;

        //uses distanceSquared as the radius of the circle, and if that distance is less than the followRange squared, will begin to follow the player.
        if (distanceSquared < followRange * followRange)
        {
            //makes the hypotenuse equal to 1
            direction.Normalize();

            rb.linearVelocity = direction * followSpeed + offset;
            offset *= 0.85f;
        }
        else
        {
            //if the fly cannot see the player, it will not move.
            rb.linearVelocity = Vector2.zero;
        }
        //transforms the fly in the direction it is moving
        if (rb.linearVelocityX > 0f)
        {
            transform.localScale = new(-3, 3);
        }
        else
        {
            transform.localScale = new(3, 3);
        }


    }

    void OnCollisionEnter2D(Collision2D collision)
    { 
        //checks collision with the players attack
        if (collision.collider.CompareTag("PlayerSwing"))
        {
            //gives an offset based on the direction the player was facing when attacking
            if (Swing.lastDirection)
            {
                offset = new(30f, 0);
            }

            else
            {
                offset = new(-30f, 0);
            }
        }
    }
}