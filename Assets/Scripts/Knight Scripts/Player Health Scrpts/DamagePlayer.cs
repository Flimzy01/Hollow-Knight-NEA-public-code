using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the player collides with an enemy or a spike they will lose one health
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            
        }

    }
}
