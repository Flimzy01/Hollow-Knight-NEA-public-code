using UnityEngine;

public class bugMove : MonoBehaviour
{
    private float directionX;
    private Rigidbody2D rb;
    private bool facingRight;
    private Vector3 localScale;
    private Vector2 offset = new(0, 0);
    void Start()
    {
        //transforms the tiktik by the local scale to ensure that it is facing the correct direction
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        directionX = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the tiktik hits the wall, it will turn around and begin moving the other way
        if (other.gameObject.CompareTag("Wall"))
        {
            directionX *= -1f;
        }



    }
    void FixedUpdate()
    {
        //adds velocity to the tiktik, will add an offset if the player hits it
        rb.linearVelocity = new Vector2(directionX * 2f, 0) + offset;
        offset *= 0.85f;
        
        FacingDirection();
    }

    void FacingDirection()
    {
        //finds the last direction of the bug to flip the sprite to travel in the opposite direction
        if (directionX > 0)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }

        if ((facingRight) && (localScale.x < 0) || (!facingRight) && (localScale.x > 0))
        {
            localScale.x *= -1f;

            transform.localScale = localScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //checks if the player hits the tiktik, and if it does adds a velocity which is quickly reduced to being insignificant
        if (collision.collider.CompareTag("PlayerSwing"))
        {
            //uses the last player direction to find which way the offset should act
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
